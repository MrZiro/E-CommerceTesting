using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result<None>> SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}
