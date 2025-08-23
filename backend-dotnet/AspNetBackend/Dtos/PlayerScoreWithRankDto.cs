namespace AspNetBackend.Dtos;

public class PlayerScoreWithRankDto
{
    public int Rank { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public long TotalScore { get; set; }
    public long TotalTime { get; set; }
    public int SessionsPlayed { get; set; }
}