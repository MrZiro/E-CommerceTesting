namespace MyCommerce.Application.Authentication;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    List<string> Roles);
