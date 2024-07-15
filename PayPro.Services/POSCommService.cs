using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using System.Text;
using PayPro.Contracts.Models.Payment;
using PayPro.Contracts.Models.POS;
using CommunityToolkit.Mvvm.Messaging;
using PayPro.Contracts.Messages;
using PayPro.Services.Interfaces;
using System.Transactions;

namespace PayPro.Services
{
    public class POSCommService : BackgroundService
    {
        private readonly ILogger<POSCommService> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private SerialPort _serialPort;

        public POSCommService(
            ILogger<POSCommService> logger,
            IPaymentService paymentService,
            IConfiguration configuration)
        {
            _logger = logger;
            _paymentService = paymentService;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serial Communication Service is starting.");

            var portName = _configuration.GetValue<string>("SerialPortConfig:PortName");
            var baudRate = _configuration.GetValue<int>("SerialPortConfig:BaudRate");
            var dataBits = _configuration.GetValue<int>("SerialPortConfig:DataBits");
            var parity = Enum.Parse<Parity>(_configuration.GetValue<string>("SerialPortConfig:Parity"));
            var stopBits = Enum.Parse<StopBits>(_configuration.GetValue<string>("SerialPortConfig:StopBits"));

            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            try
            {
                _serialPort.Open();
                _logger.LogInformation($"Serial port {portName} opened successfully.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var packet = await ReadPacketAsync(stoppingToken);
                        if (!string.IsNullOrEmpty(packet))
                        {
                            await ProcessPacketAsync(packet);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while reading from serial port");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Serial Communication Service");
            }
            finally
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _logger.LogInformation($"Serial port {portName} closed.");
                }
            }
        }

        private async Task<string> ReadPacketAsync(CancellationToken cancellationToken, int timeoutMilliseconds = -1)
        {
            var buffer = new byte[4096];
            var packet = new StringBuilder();
            bool startFound = false;

            CancellationToken combinedToken = cancellationToken;

            if (timeoutMilliseconds > 0)
            {
                var timeoutCts = new CancellationTokenSource(timeoutMilliseconds);
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                combinedToken = linkedCts.Token;
            }

            try
            {
                while (!combinedToken.IsCancellationRequested)
                {
                    int bytesRead;
                    try
                    {
                        bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, combinedToken);
                    }
                    catch (OperationCanceledException)
                    {
                        if (timeoutMilliseconds > 0 && !cancellationToken.IsCancellationRequested)
                        {
                            return string.Empty;
                        }
                        throw; 
                    }

                    for (int i = 0; i < bytesRead; i++)
                    {
                        char c = (char)buffer[i];
                        if (c == '\u0002') // STX
                        {
                            startFound = true;
                            packet.Clear();
                        }

                        if (startFound)
                        {
                            packet.Append(c);
                            if (c == '\n' && packet.Length > 1 && packet[packet.Length - 2] == '\r')
                            {
                                return packet.ToString();
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private async Task ProcessPacketAsync(string packetData)
        {
            _logger.LogInformation($"Received packet: {packetData}");

            var requestFromPOS = PaymentRequestPacket.FromPacketString(packetData);

            WeakReferenceMessenger.Default.Send(new AddRecordMessage((requestFromPOS, null)));

            var requestFromPayPro = new PaymentRequest()
            {
                TransactionId = requestFromPOS.TransactionId,
                Amount = requestFromPOS.Amount,
                PaymentIdentifier = requestFromPOS.PaymentIdentifier,
                MerchantId = requestFromPOS.LocationId,
            };

            PaymentResponse response = await _paymentService.ProcessPaymentAsync(requestFromPayPro);

            WeakReferenceMessenger.Default.Send(new AddRecordMessage((requestFromPOS, response)));

            // Send response packet to POS
            var packetResponse = new PaymentResponsePacket()
            {
                PacketType = "PAYMENT_RESPONSE",
                TransactionId = requestFromPOS.TransactionId,
                ResponseCode = response.Result == PaymentResult.Success ? "00" : "01",
                Message = response.Message.ToUpper(),
                ApprovedAmount = requestFromPOS.Amount,
                Timestamp = DateTime.Now,
                LocationId = requestFromPOS.LocationId,
                ApprovalCode = response.ApprovalCode,
                Issuer = requestFromPOS.PaymentMethod,
                LastFourDigits = requestFromPOS.PaymentIdentifier[^4..],
                POSTerminalId = requestFromPOS.POSTerminalId
            };

            var packetResponseString = packetResponse.GeneratePacket();
            _serialPort.Write(packetResponseString);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serial Communication Service is stopping.");

            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}