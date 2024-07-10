using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PayPro.Contracts.Interfaces;
using PayPro.Contracts.Models.Payment;

namespace PayPro.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private const string PAYMENT_ENDPOINT = "/api/payment";

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    PropertyNameCaseInsensitive = true
                }; 

                var response = await _httpClient.PostAsync(PAYMENT_ENDPOINT, content);

                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    var errorResponseBody = await response.Content.ReadAsStringAsync();
                    var paymentFailedResponse = JsonSerializer.Deserialize<PaymentResponse>(errorResponseBody, options);

                    return paymentFailedResponse;
                }

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseBody, options);

                return paymentResponse ?? throw new InvalidOperationException("Failed to deserialize payment response.");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error occurred while processing payment.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error occurred while deserializing response.", ex);
            }
        }
    }
}