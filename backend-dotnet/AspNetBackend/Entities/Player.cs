using System.ComponentModel.DataAnnotations;

namespace AspNetBackend.Entities;

public class Player
{
    public long Id { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? PasswordHash { get; set; } = string.Empty;

    [StringLength(200)]
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? UserEmail { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}
