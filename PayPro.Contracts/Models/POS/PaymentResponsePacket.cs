using System.Text;

namespace PayPro.Contracts.Models.POS
{
    public class PaymentResponsePacket
    {
        public string PacketType { get; set; } // "PAYMENT_RESPONSE"
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; } // 예: "00" (승인), "01" (거절) 등
        public string ResponseMessage { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public string LocationId { get; set; }
        public string ApprovalCode { get; set; } // 승인 시 발급되는 코드
        public string CardIssuer { get; set; }
        public string LastFourDigits { get; set; } // 카드 번호 마지막 4자리
        public string POSTerminalId { get; set; }

        public string GeneratePacket()
        {
            StringBuilder packetBuilder = new();

            packetBuilder.Append(AsciiCC.STX);
            packetBuilder.Append(PacketType);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(TransactionId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(ResponseCode);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(ResponseMessage);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(Timestamp.ToString("yyyyMMddHHmmss"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(ApprovalCode ?? "");
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(CardIssuer ?? "");
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(LastFourDigits ?? "");
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(ApprovedAmount.ToString("F2"));
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(POSTerminalId);
            packetBuilder.Append(AsciiCC.FS);
            packetBuilder.Append(LocationId);
            packetBuilder.Append(AsciiCC.ETX);

            // 체크섬 추가
            string checksum = PaymentPacketUtils.CalculateChecksum(packetBuilder.ToString());
            packetBuilder.Append(checksum);
            packetBuilder.Append("\r\n");

            return packetBuilder.ToString();
        }

        public static PaymentResponsePacket FromPacketString(string packetString)
        {
            var packet = new PaymentResponsePacket();

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

            if (fields.Length != 11) // 예상되는 필드 수 확인
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
            packet.ResponseCode = fields[2];
            packet.ResponseMessage = fields[3];
            packet.Timestamp = DateTime.ParseExact(fields[4], "yyyyMMddHHmmss", null);
            packet.ApprovalCode = fields[5];
            packet.CardIssuer = fields[6];
            packet.LastFourDigits = fields[7];
            packet.ApprovedAmount = decimal.Parse(fields[8]);
            packet.POSTerminalId = fields[9];
            packet.LocationId = fields[10];

            return packet;
        }
    }
}
