using Mapster;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Application.Orders.Dtos;

public record OrderDto(
    Guid Id,
    Guid UserId,
    string? UserEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    List<OrderItemDto> Items);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
