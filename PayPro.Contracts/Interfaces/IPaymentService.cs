using PayPro.Contracts.Models.Payment;

namespace PayPro.Contracts.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}
