using System.Text.RegularExpressions;
using MyCommerce.Domain.Common;
using MyCommerce.Domain.Errors;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Domain.ValueObjects;

public sealed class Sku : ValueObject
{
    private const int MaxLength = 50;
    private static readonly Regex ValidCharsRegex = new("^[a-zA-Z0-9-]*$", RegexOptions.Compiled);

    public string Value { get; private set; }

    private Sku(string value)
    {
        Value = value;
    }

    public static Result<Sku> From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Fail<Sku>(DomainErrors.Sku.Empty);
        }

        if (value.Length > MaxLength)
        {
            return Result.Fail<Sku>(DomainErrors.Sku.TooLong);
        }

        if (!ValidCharsRegex.IsMatch(value))
        {
            return Result.Fail<Sku>(DomainErrors.Sku.InvalidCharacters);
        }

        return new Sku(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
