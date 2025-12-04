using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Common.Interfaces.Authentication;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Authentication;

public class AuthService
{
    private readonly IAppDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthService(
        IAppDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<AuthResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Validate Email Value Object
        var emailResult = Email.From(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Fail<AuthResult>(emailResult.Errors);
        }

        // 2. Check if user exists
        var userExists = await _context.Users.AnyAsync(u => u.Email.Value == request.Email, cancellationToken);
        if (userExists)
        {
            return Result.Fail<AuthResult>(new Error("User.DuplicateEmail", "Email already in use."));
        }

        // 3. Hash Password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 4. Create User
        var userResult = User.Create(
            request.FirstName,
            request.LastName,
            emailResult.Value,
            passwordHash);

        if (userResult.IsFailure)
        {
            return Result.Fail<AuthResult>(userResult.Errors);
        }

        var user = userResult.Value;

        // 5. Save
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Generate Token
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            token);
    }

    public async Task<Result<AuthResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Find User
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Fail<AuthResult>(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        // 2. Validate Password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Fail<AuthResult>(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        // 3. Generate Token
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            token);
    }

    public async Task<Result<None>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken);
        
        // Security: Don't reveal if user exists. Return success even if not found.
        if (user is null)
        {
            return Result.Success(None.Value);
        }

        // Generate simple token (in prod, use cryptographically secure random string)
        var token = Guid.NewGuid().ToString("N");
        var expiry = DateTime.UtcNow.AddHours(1);

        user.SetPasswordResetToken(token, expiry);
        
        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(user.Email.Value, token, cancellationToken);

        return Result.Success(None.Value);
    }

    public async Task<Result<None>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Fail<None>(new Error("Auth.InvalidRequest", "Invalid request."));
        }

        if (user.PasswordResetToken != request.Token || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            return Result.Fail<None>(new Error("Auth.InvalidToken", "Invalid or expired token."));
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        
        var result = user.ResetPassword(newPasswordHash);
        if (result.IsFailure)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(None.Value);
    }

    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Validate Email
        var emailResult = Email.From(request.Email);
        if (emailResult.IsFailure) return Result.Fail<Guid>(emailResult.Errors);

        // 2. Check Duplicates
        var exists = await _context.Users.AnyAsync(u => u.Email.Value == request.Email, cancellationToken);
        if (exists) return Result.Fail<Guid>(new Error("User.DuplicateEmail", "Email already exists."));

        // 3. Hash Password
        var hash = _passwordHasher.HashPassword(request.Password);

        // 4. Create
        var userResult = User.Create(
            request.FirstName,
            request.LastName,
            emailResult.Value,
            hash,
            request.Roles);

        if (userResult.IsFailure) return Result.Fail<Guid>(userResult.Errors);

        _context.Users.Add(userResult.Value);
        await _context.SaveChangesAsync(cancellationToken);

        return userResult.Value.Id;
    }
}
