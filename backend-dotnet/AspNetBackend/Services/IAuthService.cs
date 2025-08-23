using System;
using AspNetBackend.Dtos;
using AspNetBackend.Entities;

namespace AspNetBackend.Services;

public interface IAuthService
{
    Task<Player?> RegisterAsync(PlayerDto request);
    Task<TokenResponseDto?> LoginAsync(PlayerDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    
}
