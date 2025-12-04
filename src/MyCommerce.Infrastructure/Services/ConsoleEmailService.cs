using Microsoft.Extensions.Logging;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

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
        _logger.LogInformation("----------------------------------------------------------------");
        _logger.LogInformation(" Sending Password Reset Email to: {Email}", email);
        _logger.LogInformation(" Token: {Token}", token);
        _logger.LogInformation("----------------------------------------------------------------");

        return Task.FromResult(Result.Success(None.Value));
    }
}
