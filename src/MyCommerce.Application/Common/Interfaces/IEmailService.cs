using MyCommerce.Domain.Common.Result;
using MyCommerce.Application.Orders.Dtos;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result<None>> SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<Result<None>> SendOrderConfirmationAsync(User user, OrderDto order, CancellationToken cancellationToken = default);
}