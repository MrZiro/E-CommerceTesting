using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Users.Update.UpdateProfile;

public class UpdateUserProfileService
{
    private readonly IAppDbContext _context;

    public UpdateUserProfileService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);

        if (user is null)
        {
            return Result.Fail(new Error("User.NotFound", "User not found."));
        }

        var result = user.UpdateProfile(request.FirstName, request.LastName);
        if (result.IsFailure)
        {
            return Result.Fail(result.Errors);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}