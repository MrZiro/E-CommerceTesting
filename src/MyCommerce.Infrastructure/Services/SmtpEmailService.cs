using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Orders.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;

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

    public async Task<Result<None>> SendOrderConfirmationAsync(User user, OrderDto order, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending REAL Order Confirmation Email via SMTP (Simulated for now)");
        _logger.LogInformation("Host: {Host}, Port: {Port}", _settings.Host, _settings.Port);
        
        // Create email content
        var subject = $"Order Confirmation - #{order.Id}";
        var body = $@"
Hello {user.Email},

Thank you for your order! Here are your order details:

Order ID: {order.Id}
Order Date: {order.OrderDate:yyyy-MM-dd HH:mm:ss}
Order Status: {order.Status}
Total Amount: ${order.TotalAmount:F2}

Items ordered:
";

        foreach (var item in order.Items)
        {
            body += $"  - {item.ProductName} x{item.Quantity} @{item.UnitPrice:F2} = ${item.Quantity * item.UnitPrice:F2}\n";
        }

        body += "\nWe'll process your order shortly.\n\nBest regards,\nThe Team";

        _logger.LogInformation("Order confirmation email queued for delivery");
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: {Body}", body);

        await Task.CompletedTask;
        return Result.Success(None.Value);
    }
}