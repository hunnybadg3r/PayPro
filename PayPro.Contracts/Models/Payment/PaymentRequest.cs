namespace PayPro.Contracts.Models.Payment
{
    public class PaymentRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string MerchantId { get; set; }
    }
}
