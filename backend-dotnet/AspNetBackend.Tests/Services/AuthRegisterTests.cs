using AspNetBackend.Dtos;
using AspNetBackend.Services;
using Microsoft.AspNetCore.Identity;

namespace AspNetBackend.Tests.Services;

public class AuthRegisterTests : AuthTestBase
{
    [Fact]
    public async Task RegisterAsync_ShouldSucceed()
    {
        // register a new player to set up the test
        var registerdPlayer1 = await _authService.RegisterAsync
        (
            new PlayerDto { Name = "user1", Password = "pw12323" }
        );
        
        var verified_result = ((AuthService)_authService).VerifyPassword(registerdPlayer1!, "pw12323");
        // returns Success if the password was correctly hashed and verified
        Assert.Equal(PasswordVerificationResult.Success, verified_result);
        /// since a user with the name "user1" was registered, these assertions should pass
        Assert.NotNull(registerdPlayer1);
        Assert.Equal("user1", registerdPlayer1?.Name);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenNameAlreadyExists()
    {
        // register a new player to set up the test
        var registerdPlayer1 = await _authService.RegisterAsync
        (
            new PlayerDto { Name = "user1", Password = "pw12323" }
        );

        // this assertion should pass because the user did not exist before
        Assert.NotNull(registerdPlayer1);

        // attempt to register the same user again
        var samePlayer = await _authService.RegisterAsync
        (
            new PlayerDto { Name = "user1", Password = "pw12323" }
        );
        // this should return null because the username "user1" already exists
        Assert.Null(samePlayer);
    }

    [Theory]
    [InlineData("us", "pw123456")]          // name is too short
    [InlineData("validname", "123")]        // password is too short
    [InlineData("", "pw123456")]            // empty name
    [InlineData("validname", "")]           // empty password
    public async Task RegisterAsync_ShouldFail_WithInvalidDto(string name, string password)
    {
        // create a PlayerDto with invalid input data
        var dto = new PlayerDto { Name = name, Password = password };
        // attempt to register the player; expect failure (null result)
        var failedRegistration = await _authService.RegisterAsync(dto);
        // assert that registration fails due to invalid data
        Assert.Null(failedRegistration);
    }
}
