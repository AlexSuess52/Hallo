using AspNetBackend.Data;
using AspNetBackend.Dtos;
using AspNetBackend.Entities;
using AspNetBackend.ValidationHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AspNetBackend.Services;


/// <summary>
/// Service handling user registration, authentication, and token management.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AspNetPostgresDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<Player> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly int _refreshTokenLifetimeDays;

    public AuthService(
        AspNetPostgresDbContext context,
        IConfiguration configuration,
        IPasswordHasher<Player> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenLifetimeDays = GetRefreshTokenLifetime();

    }

    /// <summary>
    /// Registers a new player if the username is available and input is valid.
    /// </summary>
    /// <param name="request">The player registration request.</param>
    /// <returns>The created player, or null if validation fails or username is taken.</returns>
    public async Task<Player?> RegisterAsync(PlayerDto request)
    {
        if (!Validate.IsValidDto(request, out var validationErrors))
            return null;

        if (await _context.Players.AnyAsync(p => p.Name == request.Name))
            return null;

        var player = new Player();
        player.Name = request.Name;
        player.PasswordHash = _passwordHasher.HashPassword(player, request.Password);

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return player;
    }

    /// <summary>
    /// Authenticates a player and returns access and refresh tokens if successful.
    /// </summary>
    /// <param name="request">The login request containing username and password.</param>
    /// <returns>The token response if login is successful; otherwise, null.</returns>
    public async Task<TokenResponseDto?> LoginAsync(PlayerDto request)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == request.Name);
        if (player == null)
        {
            return null;
        }

        if (VerifyPassword(player, request.Password) == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponse(player);
    }

    /// <summary>
    /// Verifies a player's password against the stored hash.
    /// </summary>
    /// <param name="player">The player entity.</param>
    /// <param name="plainPassword">The plain text password to verify.</param>
    /// <returns>The password verification result, or null if invalid data.</returns>
    internal PasswordVerificationResult? VerifyPassword(Player player, string plainPassword)
    {
        if (player == null || string.IsNullOrEmpty(player.PasswordHash))
            return null;

        return _passwordHasher.VerifyHashedPassword(player, player.PasswordHash, plainPassword);
    }

    /// <summary>
    /// Refreshes the tokens if the provided refresh token is valid and not expired.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>The new token response if successful; otherwise, null.</returns>
    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var player = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
        if (player == null)
        {
            return null;
        }
        return await CreateTokenResponse(player);
    }

    /// <summary>
    /// Generates an access and refresh token for the specified player.
    /// </summary>
    /// <param name="player">The authenticated player.</param>
    /// <returns>A token response containing access and refresh tokens.</returns>
    private async Task<TokenResponseDto> CreateTokenResponse(Player? player)
    {
        return new TokenResponseDto
        {
            AccessToken = _tokenService.CreateToken(player),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(player)
        };
    }
    
    /// <summary>
    /// Validates the provided refresh token for a specific player.
    /// </summary>
    /// <param name="player_id">The player's ID.</param>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <returns>The player if the token is valid; otherwise, null.</returns>
    private async Task<Player?> ValidateRefreshTokenAsync(long player_id, string refreshToken)
    {
        var player = await _context.Players.FindAsync(player_id);
        if (player == null || player.RefreshToken != refreshToken
            || player.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }
        return player;
    }
    
    /// <summary>
    /// Generates a new refresh token, updates the player entity, and saves changes.
    /// </summary>
    /// <param name="player">The player entity.</param>
    /// <returns>The newly generated refresh token.</returns>
    private async Task<string> GenerateAndSaveRefreshTokenAsync(Player player)
    {
        var refreshToken = _tokenService.GenerateRefreshToken();
        player.RefreshToken = refreshToken;
        player.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays);
        await _context.SaveChangesAsync();
        return refreshToken;

    }

    /// <summary>
    /// Reads the refresh token lifetime from configuration or falls back to a default value.
    /// </summary>
    /// <returns>The refresh token lifetime in days.</returns>
    private int GetRefreshTokenLifetime()
    {
        // this function is used to mock the value for the unit test
        var section = _configuration.GetSection("AppSettings:RefreshTokenLifetimeDays");
        var value = section?.Value;
        int refreshDays;
        if (!int.TryParse(value, out refreshDays))
        {
            // default value if configuration is missing or invalid
            refreshDays = 7;
        }
        return refreshDays;
    }
}
