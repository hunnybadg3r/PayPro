namespace PayPro.Contracts.Models.Payment
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public PaymentResult Result { get; set; }
        public string Message { get; set; }
        public string AuthorizationCode { get; set; }
        public DateTime TransactionDateTime { get; set; }
    }
}
