using PayPro.Contracts.Models.Payment;

namespace PayPro.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}
