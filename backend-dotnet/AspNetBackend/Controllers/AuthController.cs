using AspNetBackend.Data;
using AspNetBackend.Dtos;
using AspNetBackend.Entities;
using AspNetBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetBackend.Controllers
{
     /// <summary>
     /// API controller handling authentication and user management.
     /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AspNetPostgresDbContext _context;

        public AuthController(IAuthService authService, AspNetPostgresDbContext context) {
            _authService = authService;
            _context = context;
        }

    /// <summary>
    /// Registers a new player and returns a JWT token if successful.
    /// </summary>
    /// <param name="request">The player registration details.</param>
    /// <returns>A JWT token if registration is successful, otherwise a BadRequest.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponseDto>> Register(PlayerDto request)
    {
        var player = await _authService.RegisterAsync(request);
        if (player == null)
            return BadRequest("Username already exists or invalid input.");

        var loginResponse = await _authService.LoginAsync(request);
        if (loginResponse == null)
            return BadRequest("Token generation failed after registration.");

        return Ok(loginResponse);
    }

    /// <summary>
    /// Logs in an existing player and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="request">The login request containing username and password.</param>
    /// <returns>A JWT token if login is successful, otherwise a BadRequest.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(PlayerDto request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)

        {
            return BadRequest("Invalid username or password.");
        }

            return Ok(response);
    }

    /// <summary>
    /// Generates a new JWT token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>A new JWT token if refresh is successful, otherwise Unauthorized.</returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        if (response == null)
        {
            return Unauthorized("Invalid refresh Token");
        }

        return Ok(response);
    }

    /// <summary>
    /// Example of an authenticated-only endpoint. Requires a valid JWT token.
    /// </summary>
    /// <returns>A success message if authenticated.</returns>
    [Authorize]
    [HttpGet("authenticate")]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("Your are authenticated!");
    }


    /// <summary>
    /// Returns a list of all players. Requires authentication.
    /// </summary>
    /// <returns>A list of players with ID, name, email, and creation date.</returns>
    [Authorize]
    [HttpGet("get-all-players")]
    public IActionResult GetPlayersEndpoint()
    {
        var players = _context.Players
            .Select(p => new { p.Id, p.Name, p.UserEmail, p.CreatedAt })
            .ToList();

        return Ok(players);
    }
    }
}
