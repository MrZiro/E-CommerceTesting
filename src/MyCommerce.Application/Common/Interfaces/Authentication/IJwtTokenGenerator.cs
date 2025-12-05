using MyCommerce.Domain.Entities;

namespace MyCommerce.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
