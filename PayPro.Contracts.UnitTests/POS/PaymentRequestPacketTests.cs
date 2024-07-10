using PayPro.Contracts.Models.POS;

namespace PayPro.Contracts.UnitTests.POS;

public class PaymentRequestPacketTests
{
    public PaymentRequestPacket GetPaymentRequestPacket()
    {
        var paymentRequestPacket = new PaymentRequestPacket
        {
            PacketType = "PAYMENT_REQ",
            TransactionId = "0624-000001",
            Timestamp = new DateTime(2024, 6, 26, 14, 30, 0),
            LocationId = "HD001",
            POSTerminalId = "POS001",
            OperatorId = "OP123",
            FuelType = "GASOLINE",
            Amount = 50000.50m,
            UnitPrice = 1652.89m,
            PaymentMethod = "CREDIT_CARD", // DIGITAL_WALLET
            CardNumber = "1234567890123456",
        };

        return paymentRequestPacket;
    }
    
    [Fact]
    public void GeneratePacket_WhenGeneratedAndParsed_ShouldPreserveAllPropertiesAndChecksum()
    {
        // Arrange
        var originalPacket = GetPaymentRequestPacket();

        // Act
        string packetString = originalPacket.GeneratePacket();
        PaymentRequestPacket reconstructedPacket = PaymentRequestPacket.FromPacketString(packetString);

        // Assert
        Assert.Equal(originalPacket.PacketType, reconstructedPacket.PacketType);
        Assert.Equal(originalPacket.TransactionId, reconstructedPacket.TransactionId);
        Assert.Equal(originalPacket.Timestamp, reconstructedPacket.Timestamp);
        Assert.Equal(originalPacket.LocationId, reconstructedPacket.LocationId);
        Assert.Equal(originalPacket.POSTerminalId, reconstructedPacket.POSTerminalId);
        Assert.Equal(originalPacket.OperatorId, reconstructedPacket.OperatorId);
        Assert.Equal(originalPacket.FuelType, reconstructedPacket.FuelType);
        Assert.Equal(originalPacket.Amount, reconstructedPacket.Amount);
        Assert.Equal(originalPacket.UnitPrice, reconstructedPacket.UnitPrice);
        Assert.Equal(originalPacket.Volume, reconstructedPacket.Volume);
        Assert.Equal(originalPacket.PaymentMethod, reconstructedPacket.PaymentMethod);
        Assert.Equal(originalPacket.CardNumber, reconstructedPacket.CardNumber);

        // 체크섬 검증
        string originalChecksum = packetString.Substring(packetString.IndexOf((char)0x03) + 1).Trim();
        string recalculatedChecksum = PaymentPacketUtils.CalculateChecksum(packetString.Substring(0, packetString.IndexOf((char)0x03) + 1));
        Assert.Equal(originalChecksum, recalculatedChecksum);

        // 재생성된 패킷 문자열이 원본과 동일한지 확인
        string regeneratedPacketString = reconstructedPacket.GeneratePacket();
        Assert.Equal(packetString, regeneratedPacketString);
    }

    [Fact]
    public void FromPacketString_WhenParsedWithInvalidChecksum_ShouldThrowException()
    {
        // Arrange
        var originalPacket = GetPaymentRequestPacket();

        // Act
        string packetString = originalPacket.GeneratePacket();

        // 패킷의 마지막 2자리(체크섬)를 잘못된 값으로 변경
        string invalidPacketString = packetString.Substring(0, packetString.Length - 4) + "00\r\n";

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            PaymentRequestPacket.FromPacketString(invalidPacketString);
        });

        Assert.Contains("Checksum mismatch", exception.Message);
    }
}