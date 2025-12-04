using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IEnumerable<IPaymentStrategy> _strategies;

    public PaymentService(IEnumerable<IPaymentStrategy> strategies)
    {
        _strategies = strategies;
    }

    // Default fallback if no provider specified (mock behavior or error)
    // In our updated design, we should pass the provider name.
    // But IPaymentService interface signature is currently:
    // ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken)
    // We need to update the interface or overload it.
    // For now, let's assume a default strategy (e.g., Stripe) or update the interface.
    
    // Decision: Update IPaymentService to accept providerName.
    public async Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        // Fallback to Stripe if not specified (Legacy support)
        return await ProcessPaymentAsync(userId, amount, currency, "stripe", cancellationToken);
    }

    public async Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, string provider, CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.FirstOrDefault(s => s.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase));
        
        if (strategy is null)
        {
            return Result.Fail<bool>(new Error("Payment.InvalidProvider", $"Payment provider '{provider}' is not supported."));
        }

        return await strategy.ProcessPaymentAsync(userId, amount, currency, cancellationToken);
    }
}
