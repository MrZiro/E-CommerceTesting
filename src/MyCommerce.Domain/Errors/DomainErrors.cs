using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Domain.Errors;

public static class DomainErrors
{
    public static class Money
    {
        public static Error NegativeAmount => new("Money.NegativeAmount", "Amount cannot be negative.");
        public static Error EmptyCurrency => new("Money.EmptyCurrency", "Currency cannot be empty.");
    }
    
    // Add other domain-specific errors here
    public static class Product
    {
        public static Error NotFound => new("Product.NotFound", "Product not found.");
        public static Error NameRequired => new("Product.NameRequired", "Product name is required.");
        public static Error NameTooLong => new("Product.NameTooLong", "Product name cannot exceed 100 characters.");
        public static Error InvalidPrice => new("Product.InvalidPrice", "Product price must be greater than zero.");
        public static Error InvalidSku => new("Product.InvalidSku", "Invalid SKU format.");
        public static Error InvalidStockChange => new("Product.InvalidStockChange", "Stock cannot be negative after change.");
        public static Error CannotDeleteInUse => new("Product.CannotDeleteInUse", "Cannot delete product because it is part of existing orders.");
    }

    public static class CartItem
    {
        public static Error EmptyCartId => new("CartItem.EmptyCartId", "Cart ID cannot be empty.");
        public static Error EmptyProductId => new("CartItem.EmptyProductId", "Product ID cannot be empty.");
        public static Error InvalidQuantity => new("CartItem.InvalidQuantity", "Quantity must be greater than zero.");
        public static Error InvalidResultingQuantity => new("CartItem.InvalidResultingQuantity", "Resulting quantity must be greater than zero.");
    }

    public static class Order
    {
        public static Error EmptyUserId => new("Order.EmptyUserId", "User ID cannot be empty.");
        public static Error NoItems => new("Order.NoItems", "Order must contain at least one item.");
        public static Error EmptyStatus => new("Order.EmptyStatus", "Order status cannot be empty.");
        public static Error InvalidStatus => new("Order.InvalidStatus", "Invalid order status.");
        public static Error DuplicateItem => new("Order.DuplicateItem", "Item already exists in order.");
        public static Error MixedCurrencies => new("Order.MixedCurrencies", "All order items must use the same currency.");
        public static Error CurrencyMismatch => new("Order.CurrencyMismatch", "New item currency must match existing items.");
    }

    public static class Sku
    {
        public static Error Empty => new("Sku.Empty", "SKU cannot be empty.");
        public static Error TooLong => new("Sku.TooLong", "SKU cannot be longer than 50 characters.");
        public static Error InvalidCharacters => new("Sku.InvalidCharacters", "SKU contains invalid characters.");
    }
    
    public static class Email
    {
        public static Error Empty => new("Email.Empty", "Email cannot be empty.");
        public static Error InvalidFormat => new("Email.InvalidFormat", "Email format is invalid.");
    }

    public static class Token
    {
        public static Error Required => new("Token.Required", "Reset token is required.");
    }
}
