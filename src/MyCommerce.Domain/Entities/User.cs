using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Domain.Entities;

public sealed class User : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public List<string> Roles { get; private set; } = new();
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    // Private constructor for EF Core
    private User() 
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        PasswordHash = null!;
        Roles = new();
    }

    private User(
        Guid id,
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        List<string> roles,
        string? passwordResetToken = null,
        DateTime? passwordResetTokenExpiry = null)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Roles = roles;
        PasswordResetToken = passwordResetToken;
        PasswordResetTokenExpiry = passwordResetTokenExpiry;
    }

    public static Result<User> Create(
        string firstName,
        string lastName,
        Email email, // Expects an already validated Email Value Object
        string passwordHash,
        List<string>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Fail<User>(new Error("User.EmptyFirstName", "First name cannot be empty."));
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Fail<User>(new Error("User.EmptyLastName", "Last name cannot be empty."));
        }
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Fail<User>(new Error("User.EmptyPassword", "Password cannot be empty."));
        }
        
        // Default to Customer if no roles provided
        var userRoles = roles ?? new List<string> { "Customer" };

        return new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            passwordHash,
            userRoles);
    }

    public Result<None> UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Fail<None>(new Error("User.EmptyFirstName", "First name cannot be empty."));
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Fail<None>(new Error("User.EmptyLastName", "Last name cannot be empty."));
        }

        FirstName = firstName;
        LastName = lastName;
        return Result.Success(None.Value);
    }

    public Result<None> ChangeEmail(Email newEmail)
    {
        // Add logic to check if email already exists in DB (Application layer responsibility)
        Email = newEmail;
        return Result.Success(None.Value);
    }

    public Result<None> ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return Result.Fail<None>(new Error("User.EmptyPassword", "Password cannot be empty."));
        }
        PasswordHash = newPasswordHash;
        return Result.Success(None.Value);
    }

    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }

    public Result<None> ResetPassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return Result.Fail<None>(new Error("User.EmptyPassword", "Password cannot be empty."));
        }
        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        return Result.Success(None.Value);
    }
}