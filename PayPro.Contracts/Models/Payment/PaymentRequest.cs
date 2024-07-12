namespace PayPro.Contracts.Models.Payment
{
    public class PaymentRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentIdentifier { get; set; }
        public string MerchantId { get; set; }
    }
}
