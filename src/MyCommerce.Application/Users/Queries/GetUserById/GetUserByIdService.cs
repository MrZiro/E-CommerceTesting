using Mapster;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Authentication; // For AuthResult or similar DTO, or we create a UserDto

namespace MyCommerce.Application.Users.Queries.GetUserById;

public record UserDto(Guid Id, string FirstName, string LastName, string Email);

public class GetUserByIdService
{
    private readonly IAppDbContext _context;

    public GetUserByIdService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);

        if (user is null)
        {
            return Result.Fail<UserDto>(new Error("User.NotFound", "User not found."));
        }

        return new UserDto(user.Id, user.FirstName, user.LastName, user.Email.Value);
    }
}
