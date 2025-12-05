using Mapster;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Application.Carts.Dtos;

public record CartDto(
    Guid Id,
    Guid UserId,
    List<CartItemDto> Items,
    decimal TotalAmount,
    string Currency);

public record CartItemDto(
    Guid ProductId,
    string ProductName,
    string ProductImageUrl,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    decimal TotalPrice);
