namespace MyCommerce.Domain.Common.Result;

public class Result
{
    private readonly List<Error> _errors = new();

    public bool IsSuccess => _errors.Count == 0;
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    protected Result(List<Error> errors)
    {
        _errors = errors;
    }

    protected Result() { }

    public static Result Success() => new();
    public static Result Fail(Error error) => new(new List<Error> { error });
    public static Result Fail(IEnumerable<Error> errors) => new(errors.ToList());

    public static Result<TValue> Success<TValue>(TValue value) => new(value);
    public static Result<TValue> Fail<TValue>(Error error) => new(new List<Error> { error });
    public static Result<TValue> Fail<TValue>(IEnumerable<Error> errors) => new(errors.ToList());
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue value)
    {
        _value = value;
    }

    public Result(List<Error> errors) : base(errors)
    {
        _value = default;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value when result is a failure.");

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(new List<Error> { error });
    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);
}