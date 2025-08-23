namespace AspNetBackend.Entities;

public class LeaderBoardEntry
{

    public long Id { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public long TotalScore { get; set; }
    public long TotalTime { get; set; }
    public int SessionsPlayed { get; set; }
}
