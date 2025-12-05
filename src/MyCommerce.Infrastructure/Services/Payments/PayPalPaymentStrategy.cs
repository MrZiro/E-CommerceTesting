using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Infrastructure.Services.Payments;

public class PayPalPaymentStrategy : IPaymentStrategy
{
    public string ProviderName => "paypal";

    public Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would use PayPal Checkout SDK to capture an order
        // For now, we simulate success
        return Task.FromResult(Result.Success(true));
    }
}
