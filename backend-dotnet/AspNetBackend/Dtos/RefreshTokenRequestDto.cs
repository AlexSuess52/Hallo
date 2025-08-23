using System.ComponentModel.DataAnnotations;

namespace AspNetBackend.Dtos;

public class RefreshTokenRequestDto
{
    [Required]
    public long Id { get; set; }
    
    [Required]
    public required string RefreshToken { get; set; }
    

}
