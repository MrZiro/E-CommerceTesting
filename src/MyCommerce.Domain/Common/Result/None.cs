namespace MyCommerce.Domain.Common.Result;

// Represents the absence of a value, useful for Result<T> when T is void.
public readonly struct None
{
    public static None Value => default;
}
