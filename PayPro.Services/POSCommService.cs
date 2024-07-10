using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using System.Text;
using PayPro.Contracts.Interfaces;
using PayPro.Contracts.Models.Payment;
using PayPro.Contracts.Models.POS;
using CommunityToolkit.Mvvm.Messaging;
using PayPro.Contracts.Messages;

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

        private async Task<string> ReadPacketAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];
            var packet = new StringBuilder();
            bool startFound = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

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
                CardNumber = requestFromPOS.CardNumber,
                MerchantId = requestFromPOS.LocationId,
            };

            PaymentResponse response = await _paymentService.ProcessPaymentAsync(requestFromPayPro);

            WeakReferenceMessenger.Default.Send(new AddRecordMessage((requestFromPOS, response)));

            // TODO: Send response packet to POS
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