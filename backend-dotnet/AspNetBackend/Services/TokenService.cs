using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AspNetBackend.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AspNetBackend.Services;

/// <summary>
/// Service responsible for generating JWT access tokens and refresh tokens.
/// </summary>
public class TokenService(IConfiguration configuration) : ITokenService
{
    
    /// <summary>
    /// Creates a signed JWT access token for the specified player.
    /// </summary>
    /// <param name="player">The player for whom to generate the token.</param>
    /// <returns>A signed JWT token string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JWT token secret is not configured.</exception>
    public string CreateToken(Player player)
    {
        var claims = new List<Claim>
        {
            new("player_name", player.Name),
            new("player_id", player.Id.ToString())
        };

        var token = configuration.GetValue<string>("AppSettings:Token")
            ?? throw new InvalidOperationException("JWT Token not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var jwt = new JwtSecurityToken(
            issuer: configuration["AppSettings:Issuer"],
            audience: configuration["AppSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    /// <summary>
    /// Generates a secure, random refresh token.
    /// </summary>
    /// <returns>A Base64-encoded refresh token string.</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
