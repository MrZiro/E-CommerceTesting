namespace MyCommerce.Domain.Common.Result;

public record Error(string Code, string Description);

public static class Errors
{
    public static class General
    {
        public static Error Unexpected(string? description = null) =>
            new("General.Unexpected", description ?? "An unexpected error occurred.");
    }
}
