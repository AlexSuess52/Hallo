namespace AspNetBackend.Entities;

public class QuizSession
{
    public long Id { get; set; }
    public long TotalTime { get; set; }
    public long TotalScore { get; set; }
    public long QuizId { get; set; }
    public long PlayerId { get; set; }
}

