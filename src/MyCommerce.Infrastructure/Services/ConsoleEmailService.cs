using Microsoft.Extensions.Logging;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task<Result<None>> SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(Result.Fail<None>(DomainErrors.Email.Empty));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult(Result.Fail<None>(DomainErrors.Token.Required));
        }

        _logger.LogInformation("----------------------------------------------------------------");
        _logger.LogInformation(" Sending Password Reset Email to: [REDACTED] (domain: {Domain})", email.Split('@').LastOrDefault());
        _logger.LogInformation(" Token: [REDACTED] (length: {Length})", token.Length);
        _logger.LogInformation("----------------------------------------------------------------");

        return Task.FromResult(Result.Success(None.Value));
    }
}
