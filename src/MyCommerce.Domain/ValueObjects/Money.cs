using MyCommerce.Domain.Common;
using MyCommerce.Domain.Errors;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!; // Non-nullable property, initialized in factory method

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> From(decimal amount, string currency)
    {
        if (amount < 0)
        {
            return Result.Fail<Money>(DomainErrors.Money.NegativeAmount);
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return Result.Fail<Money>(DomainErrors.Money.EmptyCurrency);
        }

        return new Money(amount, currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
