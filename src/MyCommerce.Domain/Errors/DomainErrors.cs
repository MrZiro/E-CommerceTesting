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
        public static Error NameTooLong => new("Product.NameTooLong", "Product name cannot exceed 100 characters.");
        public static Error InvalidPrice => new("Product.InvalidPrice", "Product price must be greater than zero.");
        public static Error InvalidSku => new("Product.InvalidSku", "Invalid SKU format.");
        public static Error InvalidStockChange => new("Product.InvalidStockChange", "Stock cannot be negative after change.");
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
}
