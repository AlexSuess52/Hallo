using AspNetBackend.Entities;
using AspNetBackend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

// base test class for authentication-related tests.
// inherits from DbTestBase to get an in-memory database context.
public abstract class AuthTestBase : DbTestBase
{
    protected readonly IAuthService _authService;

    public AuthTestBase() : base()
    {
        // create a real password hasher to hash and verify passwords in tests.
        var passwordHasher = new PasswordHasher<Player>();

        // mock IConfiguration to provide application settings needed by AuthService.
        var configurationMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();

        // setup mock to return "7" as RefreshToken lifetime days from configuration.
        sectionMock.Setup(s => s.Value).Returns("7");
        configurationMock.Setup(cfg => cfg.GetSection("AppSettings:RefreshTokenLifetimeDays")).Returns(sectionMock.Object);

        // mock ITokenService to avoid generating real tokens in tests.
        var tokenServiceMock = new Mock<ITokenService>();
        // setup mocked token service to always return fixed strings as tokens.
        tokenServiceMock.Setup(t => t.CreateToken(It.IsAny<Player>())).Returns("mocked-access-token");
        tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("mocked-refresh-token");

        _authService = new AuthService(
            _context,
            configurationMock.Object,
            passwordHasher,
            tokenServiceMock.Object);
    }
}
