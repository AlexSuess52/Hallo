namespace QuizDesktop.Models;

public class LeaderboardResponse
{
    public int Rank { get; set; }
    public string? PlayerName { get; set; }
    public int TotalScore { get; set; }
    public int TotalTime { get; set; }
    public int SessionsPlayed { get; set; }
}