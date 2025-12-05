using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Infrastructure.Services.Payments;

public class StripePaymentStrategy : IPaymentStrategy
{
    public string ProviderName => "stripe";

    public Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would use Stripe.net to confirm a PaymentIntent
        // For now, we simulate success
        return Task.FromResult(Result.Success(true));
    }
}
