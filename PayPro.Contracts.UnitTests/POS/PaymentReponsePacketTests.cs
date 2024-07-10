using PayPro.Contracts.Models.POS;
using Xunit;

namespace PayPro.Contracts.UnitTests.POS
{
    public class PaymentResponsePacketTests
    {
        [Fact]
        public void PaymentResponsePacket_WhenGeneratedAndParsed_ShouldPreserveAllPropertiesAndChecksum()
        {
            // Arrange
            var originalPacket = new PaymentResponsePacket
            {
                PacketType = "PAYMENT_RESPONSE",
                TransactionId = "0709-200001",
                ResponseCode = "00",
                ResponseMessage = "Approved",
                Timestamp = new DateTime(2024, 6, 26, 14, 30, 0),
                ApprovalCode = "AP123456",
                CardIssuer = "OILP",
                LastFourDigits = "1234",
                ApprovedAmount = 50000.50m,
                POSTerminalId = "POS001",
                LocationId = "HD001"
            };

            // Act
            string packetString = originalPacket.GeneratePacket();
            PaymentResponsePacket reconstructedPacket = PaymentResponsePacket.FromPacketString(packetString);

            // Assert
            Assert.Equal(originalPacket.PacketType, reconstructedPacket.PacketType);
            Assert.Equal(originalPacket.TransactionId, reconstructedPacket.TransactionId);
            Assert.Equal(originalPacket.ResponseCode, reconstructedPacket.ResponseCode);
            Assert.Equal(originalPacket.ResponseMessage, reconstructedPacket.ResponseMessage);
            Assert.Equal(originalPacket.Timestamp, reconstructedPacket.Timestamp);
            Assert.Equal(originalPacket.ApprovalCode, reconstructedPacket.ApprovalCode);
            Assert.Equal(originalPacket.CardIssuer, reconstructedPacket.CardIssuer);
            Assert.Equal(originalPacket.LastFourDigits, reconstructedPacket.LastFourDigits);
            Assert.Equal(originalPacket.ApprovedAmount, reconstructedPacket.ApprovedAmount);
            Assert.Equal(originalPacket.POSTerminalId, reconstructedPacket.POSTerminalId);
            Assert.Equal(originalPacket.LocationId, reconstructedPacket.LocationId);

            // üũ�� ����
            string originalChecksum = packetString.Substring(packetString.IndexOf((char)0x03) + 1).Trim();
            string recalculatedChecksum = PaymentPacketUtils.CalculateChecksum(packetString.Substring(0, packetString.IndexOf((char)0x03) + 1));
            Assert.Equal(originalChecksum, recalculatedChecksum);

            // ������� ��Ŷ ���ڿ��� ������ �������� Ȯ��
            string regeneratedPacketString = reconstructedPacket.GeneratePacket();
            Assert.Equal(packetString, regeneratedPacketString);
        }

        [Fact]
        public void PaymentResponsePacket_WhenParsedWithInvalidChecksum_ShouldThrowException()
        {
            // Arrange
            var originalPacket = new PaymentResponsePacket
            {
                PacketType = "PAYMENT_RESPONSE",
                TransactionId = "0709-200001",
                ResponseCode = "00",
                ResponseMessage = "Approved",
                Timestamp = new DateTime(2024, 6, 26, 14, 30, 0),
                ApprovalCode = "AP123456",
                CardIssuer = "OILP",
                LastFourDigits = "1234",
                ApprovedAmount = 50000.50m,
                POSTerminalId = "POS001",
                LocationId = "HD001"
            };

            // Act
            string packetString = originalPacket.GeneratePacket();

            // ��Ŷ�� ������ 2�ڸ�(üũ��)�� �߸��� ������ ����
            string invalidPacketString = packetString.Substring(0, packetString.Length - 4) + "00\r\n";

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                PaymentResponsePacket.FromPacketString(invalidPacketString);
            });

            Assert.Contains("Checksum mismatch", exception.Message);
        }
    }
}