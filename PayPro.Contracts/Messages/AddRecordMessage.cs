using CommunityToolkit.Mvvm.Messaging.Messages;
using PayPro.Contracts.Models.Payment;
using PayPro.Contracts.Models.POS;

namespace PayPro.Contracts.Messages
{
    public class AddRecordMessage : ValueChangedMessage<(PaymentRequestPacket packetReq, PaymentResponse payRes)>
    {
        public AddRecordMessage((PaymentRequestPacket packetReq, PaymentResponse payRes) value) : base(value)
        {
        }
    }
}
