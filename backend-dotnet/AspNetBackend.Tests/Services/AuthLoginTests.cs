using AspNetBackend.Dtos;
using AspNetBackend.Entities;
using Microsoft.AspNetCore.Identity;

namespace AspNetBackend.Tests.Services;

public class AuthLoginTests : AuthTestBase
{
    private readonly PasswordHasher<Player> _passwordHasher = new();

    [Fact]
    public async Task LoginAsync_ShouldReturnTokenResponse_WhenCredentialsAreCorrect()
    {
        // add a new player to set up the test
        var player = new Player
        {
            Name = "testuser",
            PasswordHash = _passwordHasher.HashPassword(null!, "correct_password")
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var dto = new PlayerDto { Name = "testuser", Password = "correct_password" };

        // attempt to login with the newly added player
        var result = await _authService.LoginAsync(dto);

        // The result should contain the mocked access and refresh tokens, 
        // so both tokens must not be null or empty
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result?.AccessToken));
        Assert.False(string.IsNullOrEmpty(result?.RefreshToken));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var dto = new PlayerDto { Name = "nonexistent", Password = "any" };

        var result = await _authService.LoginAsync(dto);

        // the result should be null because the player does not exist
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // add a new player to set up the test
        var player = new Player
        {
            Name = "testuser",
            PasswordHash = _passwordHasher.HashPassword(null!, "correct_password")
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var dto = new PlayerDto { Name = "testuser", Password = "wrong_password" };

        // attempt to log in with the incorrect password
        var result = await _authService.LoginAsync(dto);

        // the result should be null because the login fails due to wrong password
        Assert.Null(result);
    }
}
