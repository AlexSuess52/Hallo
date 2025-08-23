namespace QuizDesktop.Models;

public class PlayerResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public DateTime CreatedAt { get; set; }
}