using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MockPOS.Main.Local.Helper;
using PayPro.Contracts.Models.POS;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Text;

namespace MockPOS.Main.Local.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private IConfiguration _configuration;
        private SerialPort _serialPort;
        private int _transactionCount = 100001;

        private string _fuelType = "GASOLINE";
        public string FuelType
        {
            get => _fuelType;
            set
            {
                if (_fuelType != value)
                {
                    PaymentRequestPacket.FuelType = value;
                    CheckAndGeneratePacket();
                    _fuelType = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    PaymentRequestPacket.Amount = value;
                    CheckAndGeneratePacket();
                    _amount = value;
                    OnPropertyChanged();
                }
            }
        }

        private void CheckAndGeneratePacket()
        {
            if (PaymentMethod == "CREDIT_CARD")
            {
                if (PaymentRequestPacket.PaymentIdentifier.Length > 13)
                {
                    PaymentRequestPacket.Timestamp = DateTime.Now;
                    PacketStringRequest = PaymentRequestPacket.GeneratePacket();

                    UpdatePacketDisplay();
                }
            }
            else if (PaymentMethod == "DIGITAL_WALLET")
            {
                if (IdentificationToken.Length == 16)
                {
                    PaymentRequestPacket.PaymentIdentifier = IdentificationToken;
                    PaymentRequestPacket.Timestamp = DateTime.Now;
                    PacketStringRequest = PaymentRequestPacket.GeneratePacket();

                    UpdatePacketDisplay();
                }
            }
        }

        private void UpdatePacketDisplay()
        {
            DisplayRequestPacket = ControlCharacterHelper.ConvertToReadableString(PacketStringRequest);
            DisplayResponsePacket = null;
            TxTimestamp = null;
            RxTimestamp = null;
            IsPacketSent = false;
        }

        private string _paymentMethod = "CREDIT_CARD";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                if (_paymentMethod != value)
                {
                    _paymentMethod = value;
                    PaymentRequestPacket.PaymentMethod = value;
                    
                    CardNumber1 = "";
                    CardNumber2 = "";
                    CardNumber3 = "";
                    CardNumber4 = "";

                    IdentificationToken = "";

                    PaymentRequestPacket.PaymentIdentifier = "";
                    
                    DisplayRequestPacket = "";

                    OnPropertyChanged();
                }
            }
        }

        //IdentificationToken
        private string _identificationToken;
        public string IdentificationToken
        {
            get => _identificationToken;
            set
            {
                if (_identificationToken != value)
                {
                    _identificationToken = value;
                    CheckAndGeneratePacket();
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendPacketCommand))]
        private bool _isPacketSent;

        [ObservableProperty]
        private string _txTimestamp;

        [ObservableProperty]
        private string _rxTimestamp;

        [ObservableProperty]
        private PaymentRequestPacket _paymentRequestPacket;

        [ObservableProperty]
        private PaymentResponsePacket _paymentResponsePacket = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendPacketCommand))]
        private string _packetStringRequest;

        [ObservableProperty]
        private string _packetStringResponse;

        [ObservableProperty]
        private string _displayResponsePacket;

        public ObservableCollection<string> FuelTypes { get; } = new ObservableCollection<string>
        {
            "GASOLINE",
            "DIESEL"
        };

        public ObservableCollection<string> PaymentMethods { get; } = new ObservableCollection<string>
        {
            "CREDIT_CARD",
            "DIGITAL_WALLET"
        };

        private string _cardNumber1, _cardNumber2, _cardNumber3, _cardNumber4;
        private bool _isCardNumber2Enabled, _isCardNumber3Enabled, _isCardNumber4Enabled;

        private string _displayRequestPacket;
        public string DisplayRequestPacket
        {
            get => _displayRequestPacket;
            set
            {
                if (_displayRequestPacket != value)
                {
                    _displayRequestPacket = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CardNumber1
        {
            get => _cardNumber1;
            set
            {
                if (_cardNumber1 != value)
                {
                    _cardNumber1 = value;
                    OnPropertyChanged();
                    UpdateCardNumbersStates();
                }
            }
        }

        public string CardNumber2
        {
            get => _cardNumber2;
            set
            {
                if (_cardNumber2 != value)
                {
                    _cardNumber2 = value;
                    OnPropertyChanged();
                    UpdateCardNumbersStates();
                }
            }
        }

        public string CardNumber3
        {
            get => _cardNumber3;
            set
            {
                if (_cardNumber3 != value)
                {
                    _cardNumber3 = value;
                    OnPropertyChanged();
                    UpdateCardNumbersStates();
                }
            }
        }

        public string CardNumber4
        {
            get => _cardNumber4;
            set
            {
                if (_cardNumber4 != value)
                {
                    _cardNumber4 = value;
                    OnPropertyChanged();
                    UpdateCardNumbersStates();
                }
            }
        }

        public bool IsCardNumber2Enabled
        {
            get => _isCardNumber2Enabled;
            set
            {
                if (_isCardNumber2Enabled != value)
                {
                    _isCardNumber2Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCardNumber3Enabled
        {
            get => _isCardNumber3Enabled;
            set
            {
                if (_isCardNumber3Enabled != value)
                {
                    _isCardNumber3Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCardNumber4Enabled
        {
            get => _isCardNumber4Enabled;
            set
            {
                if (_isCardNumber4Enabled != value)
                {
                    _isCardNumber4Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateCardNumbersStates()
        {
            IsCardNumber2Enabled = CardNumber1?.Length == 4;
            IsCardNumber3Enabled = CardNumber2?.Length == 4;
            IsCardNumber4Enabled = CardNumber3?.Length == 4;
            UpdateFullPaymentIdentifier();
        }

        public MainWindowViewModel()
        {
            PaymentRequestPacket = new()
            {
                PacketType = "PAYMENT_REQ",
                TransactionId = $"{DateTime.Now:MMdd}-{_transactionCount:D6}",
                Timestamp = DateTime.Now,
                LocationId = "HD001",
                POSTerminalId = "POS001",
                OperatorId = "OP123",
                FuelType = "GASOLINE",
                Amount = 50000.0m,
                UnitPrice = 1652.89m,
                Volume = 30.25m,
                PaymentMethod = "CREDIT_CARD",
                PaymentIdentifier = "",
            };
        }

        public void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            var portName = _configuration.GetValue<string>("SerialPortConfig:PortName");
            var baudRate = _configuration.GetValue<int>("SerialPortConfig:BaudRate");
            var dataBits = _configuration.GetValue<int>("SerialPortConfig:DataBits");
            var parity = Enum.Parse<Parity>(_configuration.GetValue<string>("SerialPortConfig:Parity"));
            var stopBits = Enum.Parse<StopBits>(_configuration.GetValue<string>("SerialPortConfig:StopBits"));
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            _serialPort.Open();
        }

        private Random _random = new();

        [RelayCommand]
        public void RollDice()
        {
            Amount = (_random.Next(1, 16) * 10000m);
            PaymentRequestPacket.TransactionId = $"{DateTime.Now:MMdd}-{_transactionCount:D6}";
            PaymentRequestPacket.Timestamp = DateTime.Now;

            if (_random.Next(0, 2) == 0)
            {
                FuelType = "GASOLINE";
            }
            else
            {
                FuelType = "DIESEL";
            }

            if (_random.Next(0, 2) == 0)
            {
                PaymentMethod = "CREDIT_CARD";
            }
            else
            {
                PaymentMethod = "DIGITAL_WALLET";
            }

            switch (PaymentMethod)
            {
                case "CREDIT_CARD":
                    {
                        int length = _random.Next(14, 17);
                        var randomPaymentIdentifier = GenerateRandomPaymentIdentifier(length);

                        CardNumber1 = randomPaymentIdentifier.Substring(0, 4);
                        CardNumber2 = randomPaymentIdentifier.Substring(4, 4);
                        CardNumber3 = randomPaymentIdentifier.Substring(8, 4);
                        CardNumber4 = randomPaymentIdentifier.Substring(12, Math.Min(4, length - 12));
                        break;
                    }

                case "DIGITAL_WALLET":
                    {
                        IdentificationToken = TokenGenerator.GenerateToken(16);
                        break;
                    }
            }
        }

        private string GenerateRandomPaymentIdentifier(int length)
        {
            string digits = "0123456789";
            char[] PaymentIdentifier = new char[length];

            for (int i = 0; i < length; i++)
            {
                PaymentIdentifier[i] = digits[_random.Next(digits.Length)];
            }

            return new string(PaymentIdentifier);
        }

        [RelayCommand(CanExecute = nameof(CanSendPacket))]
        public async Task SendPacket()
        {
            _serialPort.Write(PacketStringRequest);
            TxTimestamp = DateTime.Now.ToString();
            IsPacketSent = true;

            _transactionCount++;
            PaymentRequestPacket.TransactionId = $"{DateTime.Now:MMdd}-{_transactionCount:D6}";

            //TODO Read response 
            var cts = new CancellationTokenSource();
            PacketStringResponse = await ReadPacketAsync(cts.Token, 60_000);
            DisplayResponsePacket = ControlCharacterHelper.ConvertToReadableString(PacketStringResponse);
            RxTimestamp = DateTime.Now.ToString();
        }

        private bool CanSendPacket()
        {
            return !string.IsNullOrEmpty(PacketStringRequest) && !IsPacketSent;
        }

        private void UpdateFullPaymentIdentifier()
        {
            PaymentRequestPacket.PaymentIdentifier = $"{CardNumber1}{CardNumber2}{CardNumber3}{CardNumber4}".Trim();

            if (PaymentRequestPacket.PaymentIdentifier.Length > 13)
            {
                PaymentRequestPacket.Timestamp = DateTime.Now;
                PacketStringRequest = PaymentRequestPacket.GeneratePacket();

                //Update Packet
                DisplayRequestPacket = ControlCharacterHelper.ConvertToReadableString(PacketStringRequest);
                DisplayResponsePacket = null;
                TxTimestamp = null;
                RxTimestamp = null;
                IsPacketSent = false;
            }
            else
            {
                PacketStringRequest = null;
                DisplayRequestPacket = null;
                DisplayResponsePacket = null;
                TxTimestamp = null;
                RxTimestamp = null;
                IsPacketSent = false;
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
                            //Time out 
                            DisplayResponsePacket = "Time out";

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
    }
}
