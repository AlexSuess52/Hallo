using System.ComponentModel.DataAnnotations;

namespace AspNetBackend.Dtos;

public record class PlayerDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public required string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public required string Password { get; set; } = string.Empty;
}
