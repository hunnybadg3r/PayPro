using PayPro.Contracts.Models.Payment;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var paymentStorage = new Dictionary<string, PaymentResponse>();

app.MapPost("/api/payment", async (PaymentRequest paymentRequest) =>
{
    Random random = new();
    await Task.Delay(random.Next(2000, 2001));

    // Validation 
    var transactionIdPattern = @"^\d{4}-\d{6}$";
    if (!Regex.IsMatch(paymentRequest.TransactionId, transactionIdPattern))
    {
        return Results.BadRequest("Invalid TransactionId format. Expected format: '20240624-000001'.");
    }

    if (paymentRequest.Amount < 1)
    {
        return Results.BadRequest("Amount must be at least 1.");
    }

    if (paymentRequest.CardNumber.Length < 14 || paymentRequest.CardNumber.Length > 16)
    {
        return Results.BadRequest("CardNumber must be between 14 and 16 characters.");
    }

    if (paymentRequest.MerchantId.Length < 5)
    {
        return Results.BadRequest("MerchantId must be at least 5 characters long.");
    }

    // Simulate random failure (1/5 chance)
    if (random.Next(1, 6) == 1) 
    {
        var paymentResponse = new PaymentResponse
        {
            TransactionId = paymentRequest.TransactionId,
            Result = PaymentResult.Declined, 
            Message = "Insufficient balance", 
            AuthorizationCode = "", 
            TransactionDateTime = DateTime.Now
        };

        return Results.Json(paymentResponse, statusCode: 422); 
    }
    else
    {
        var paymentResponse = new PaymentResponse
        {
            TransactionId = paymentRequest.TransactionId,
            Result = PaymentResult.Success, 
            Message = "Payment successful",
            AuthorizationCode = "0001",
            TransactionDateTime = DateTime.Now
        };

        paymentStorage[paymentRequest.TransactionId] = paymentResponse;

        return Results.Ok(paymentResponse);
    }
});

app.Run();
