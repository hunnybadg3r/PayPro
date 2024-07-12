using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PayPro.Contracts.Models.POS
{
    public class PaymentRequestPacket : INotifyPropertyChanged
    {
        public string PacketType { get; set; }
        private string _transactionId;
        public string TransactionId
        {
            get { return _transactionId; }
            set
            {
                if (_transactionId != value)
                {
                    _transactionId = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    OnPropertyChanged();
                }
            }
        }
        public string LocationId { get; set; }
        public string POSTerminalId { get; set; }
        public string OperatorId { get; set; }
        
        private string _fuelType;
        public string FuelType
        {
            get => _fuelType;
            set
            {
                if (_fuelType != value)
                {
                    _fuelType = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _amount;
        private decimal _unitPrice;
        private decimal _volume;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged();
                    CalculateVolume();
                }
            }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged();
                    CalculateVolume();
                }
            }
        }

        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged();
                }
            }
        }

        private void CalculateVolume()
        {
            if (UnitPrice != 0)
            {
                decimal calculatedVolume = Amount / UnitPrice;
                Volume = Math.Round(calculatedVolume, 2);
            }
        }

        private string _paymentMethod;
        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                if (_paymentMethod != value)
                {
                    _paymentMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PaymentIdentifier { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GeneratePacket()
        {
            StringBuilder packetBuilder = new();

            packetBuilder.Append(AsciiCC.STX);
            packetBuilder.Append(PacketType);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(TransactionId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(Timestamp.ToString("yyyyMMddHHmmss"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(LocationId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(POSTerminalId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(OperatorId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(FuelType);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(Amount.ToString("F2"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(UnitPrice.ToString("F2"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(Volume.ToString("F2"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(PaymentMethod);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(PaymentIdentifier ?? "");
            packetBuilder.Append(AsciiCC.ETX);

            // 체크섬 추가
            string checksum = PaymentPacketUtils.CalculateChecksum(packetBuilder.ToString());
            packetBuilder.Append(checksum);
            packetBuilder.Append("\r\n");

            return packetBuilder.ToString();
        }

        public static PaymentRequestPacket FromPacketString(string packetString)
        {
            var packet = new PaymentRequestPacket();

            // STX 제거
            string content = packetString.TrimStart(AsciiCC.STX);

            // ETX와 체크섬 분리
            int etxIndex = content.IndexOf(AsciiCC.ETX);
            if (etxIndex == -1)
            {
                throw new FormatException("Invalid packet format: ETX not found");
            }

            string checksumPart = content.Substring(etxIndex + 1).Trim('\r', '\n');
            content = content.Substring(0, etxIndex);

            // 필드 분리
            string[] fields = content.Split(AsciiCC.FS);

            if (fields.Length != 12) // 예상되는 필드 수 확인
            {
                throw new FormatException("Invalid packet format: unexpected number of fields");
            }

            // 체크섬 검증
            string calculatedChecksum = PaymentPacketUtils.CalculateChecksum(AsciiCC.STX + content + AsciiCC.ETX);
            if (calculatedChecksum != checksumPart)
            {
                throw new InvalidOperationException("Checksum mismatch");
            }

            // 필드 파싱
            packet.PacketType = fields[0];
            packet.TransactionId = fields[1];
            packet.Timestamp = DateTime.ParseExact(fields[2], "yyyyMMddHHmmss", null);
            packet.LocationId = fields[3];
            packet.POSTerminalId = fields[4];
            packet.OperatorId = fields[5];
            packet.FuelType = fields[6];
            packet.Amount = decimal.Parse(fields[7]);
            packet.UnitPrice = decimal.Parse(fields[8]);
            packet.Volume = decimal.Parse(fields[9]);
            packet.PaymentMethod = fields[10];
            packet.PaymentIdentifier = fields[11];

            return packet;
        }
    }
}
