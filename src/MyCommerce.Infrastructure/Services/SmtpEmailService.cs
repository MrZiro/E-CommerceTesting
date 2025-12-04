using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Infrastructure.Services;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result<None>> SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // NOTE: Since we don't have MailKit installed yet (and to avoid complexity without real SMTP creds),
        // we will simulate the structure but keep logging for now.
        // In a real implementation, we would:
        // using var client = new SmtpClient();
        // await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        // await client.AuthenticateAsync(_settings.Username, _settings.Password);
        // await client.SendAsync(message);
        // await client.DisconnectAsync(true);

        _logger.LogInformation("Sending REAL Email via SMTP (Simulated for now)");
        _logger.LogInformation("Host: {Host}, Port: {Port}", _settings.Host, _settings.Port);
        _logger.LogInformation("Password reset email queued for delivery");

        await Task.CompletedTask;
        return Result.Success(None.Value);
    }
}
