using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Common.Interfaces;

public interface IPaymentStrategy
{
    string ProviderName { get; }
    Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken = default);
}
