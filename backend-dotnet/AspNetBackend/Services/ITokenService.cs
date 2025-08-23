using AspNetBackend.Entities;

namespace AspNetBackend.Services;

public interface ITokenService
{
    string CreateToken(Player player);
    string GenerateRefreshToken();
}
