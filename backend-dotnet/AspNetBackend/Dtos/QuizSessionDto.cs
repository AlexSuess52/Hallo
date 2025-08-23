
using System.ComponentModel.DataAnnotations;
namespace AspNetBackend.Dtos;

public class QuizSessionDto
{

    [Required]
    public long Id { get; set; }
    [Required]
    public long TotalScore { get; set; }
    [Required]
    public long TotalTime { get; set; }
    [Required]
    public long PlayerId { get; set; }
    [Required]
    public long QuizId { get; set; }

}