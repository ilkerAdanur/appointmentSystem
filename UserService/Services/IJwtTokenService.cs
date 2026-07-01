using UserService.Models;

namespace UserService.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    DateTime GetExpirationDate();
    DateTime GetRefreshTokenExpirationDate();
}
