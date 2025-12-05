using System.Text.RegularExpressions;
using MyCommerce.Domain.Common;
using MyCommerce.Domain.Errors;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Fail<Email>(DomainErrors.Email.Empty);
        }

        if (!EmailRegex.IsMatch(value))
        {
            return Result.Fail<Email>(DomainErrors.Email.InvalidFormat);
        }

        return new Email(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
