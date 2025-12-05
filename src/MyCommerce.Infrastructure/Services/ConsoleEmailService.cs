using Microsoft.Extensions.Logging;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Orders.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
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

    public Task<Result<None>> SendOrderConfirmationAsync(User user, OrderDto order, CancellationToken cancellationToken = default)
    {
        if (user is null)
        {
            return Task.FromResult(Result.Fail<None>(DomainErrors.User.NotFound));
        }

        if (order is null)
        {
            return Task.FromResult(Result.Fail<None>(new Error("Order.Null", "Order cannot be null")));
        }

        _logger.LogInformation("----------------------------------------------------------------");
        _logger.LogInformation(" Sending Order Confirmation Email");
        _logger.LogInformation(" To: {Email}", user.Email);
        _logger.LogInformation(" Order ID: {OrderId}", order.Id);
        _logger.LogInformation(" Order Date: {OrderDate}", order.OrderDate);
        _logger.LogInformation(" Total Amount: {TotalAmount}", order.TotalAmount);
        _logger.LogInformation(" Status: {Status}", order.Status);
        _logger.LogInformation(" Items:");
        
        foreach (var item in order.Items)
        {
            _logger.LogInformation("   - {ProductName} x{Quantity} @{UnitPrice}", item.ProductName, item.Quantity, item.UnitPrice);
        }
        
        _logger.LogInformation("----------------------------------------------------------------");

        return Task.FromResult(Result.Success(None.Value));
    }
}