using AspNetBackend.Dtos;
using AspNetBackend.Entities;
using Microsoft.AspNetCore.Identity;

namespace AspNetBackend.Tests.Services;

public class AuthRefreshTokenTests : AuthTestBase
{
    private readonly PasswordHasher<Player> _passwordHasher = new();

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // add a player with a valid refresh token
        var player = new Player
        {
            Name = "refreshuser",
            PasswordHash = _passwordHasher.HashPassword(null!, "password"),
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) // noch gültig
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var requestDto = new RefreshTokenRequestDto
        {
            Id = player.Id,
            RefreshToken = "valid-refresh-token"
        };

        // call RefreshTokenAsync with the valid token
        var result = await _authService.RefreshTokenAsync(requestDto);

        // The result should contain the mocked access and refresh tokens, 
        // so both tokens must not be null or empty
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result?.AccessToken));
        Assert.False(string.IsNullOrEmpty(result?.RefreshToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsExpired()
    {
        // add a player with an expired refresh token
        var player = new Player
        {
            Name = "expireduser",
            PasswordHash = _passwordHasher.HashPassword(null!, "password"),
            RefreshToken = "expired-token",
            // -1 days means it's expired yesterday
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var requestDto = new RefreshTokenRequestDto
        {
            Id = player.Id,
            RefreshToken = "expired-token"
        };

        // call RefreshTokenAsync with the expired token
        var result = await _authService.RefreshTokenAsync(requestDto);

        // should return null since token is expired
        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        // add a player with a valid refresh token
        var player = new Player
        {
            Name = "invalidtokenuser",
            PasswordHash = _passwordHasher.HashPassword(null!, "password"),
            RefreshToken = "some-valid-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var requestDto = new RefreshTokenRequestDto
        {
            Id = player.Id,
            RefreshToken = "wrong-token"
        };

        // call RefreshTokenAsync with an invalid token
        var result = await _authService.RefreshTokenAsync(requestDto);

        // should return null since the token does not match
        Assert.Null(result);
    }
}
