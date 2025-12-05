using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, CancellationToken cancellationToken = default);
    Task<Result<bool>> ProcessPaymentAsync(Guid userId, decimal amount, string currency, string provider, CancellationToken cancellationToken = default);
}
