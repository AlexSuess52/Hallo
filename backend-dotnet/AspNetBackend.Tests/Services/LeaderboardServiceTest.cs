using AspNetBackend.Entities;
using AspNetBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace AspNetBackend.Tests.Services;

public class LeaderboardsServiceTests : DbTestBase
{
    private readonly LeaderBoardsService LeaderBoardService;

    public LeaderboardsServiceTests()
    {
        LeaderBoardService = new LeaderBoardsService(_context);
    }

    [Fact]
    public async Task GetAllQuizSessions_ReturnsAllSessions()
    {
        // insert multiple quiz sessions into the database
        _context.QuizSessions.AddRange(new[]
        {
            new QuizSession { Id = 1, PlayerId = 1, QuizId = 1, TotalScore = 10, TotalTime = 1000 },
            new QuizSession { Id = 2, PlayerId = 2, QuizId = 1, TotalScore = 15, TotalTime = 900 }
        });
        await _context.SaveChangesAsync();

        // retrieve all quiz sessions via the service
        var sessions = await LeaderBoardService.GetAllQuizSessions();

        // verify that sessions are returned correctly
        Assert.NotNull(sessions);
        Assert.Equal(2, sessions.Count);
    }

    [Fact]
    public async Task RecalculateLeaderBoard_UpdatesLeaderBoardEntriesCorrectly()
    {
        // add players and their quiz sessions
        _context.Players.AddRange(new[]
        {
            new Player { Id = 1, Name = "Alice" },
            new Player { Id = 2, Name = "Bob" }
        });

        _context.QuizSessions.AddRange(new[]
        {
            new QuizSession { PlayerId = 1, QuizId = 1, TotalScore = 10, TotalTime = 1000 },
            new QuizSession { PlayerId = 1, QuizId = 2, TotalScore = 5, TotalTime = 800 },
            new QuizSession { PlayerId = 2, QuizId = 1, TotalScore = 15, TotalTime = 900 }
        });

        await _context.SaveChangesAsync();

        // recalculate the leaderboard entries based on current data
        await LeaderBoardService.RecalculateLeaderBoard();

        // verify the leaderboard entries were created/updated correctly
        var entries = await _context.LeaderBoardEntries.ToListAsync();

        Assert.Equal(2, entries.Count);

        var alice = entries.Single(e => e.PlayerName == "Alice");
        var bob = entries.Single(e => e.PlayerName == "Bob");

        Assert.Equal(15, alice.TotalScore);
        Assert.Equal(1800, alice.TotalTime);
        Assert.Equal(2, alice.SessionsPlayed);

        Assert.Equal(15, bob.TotalScore);
        Assert.Equal(900, bob.TotalTime);
        Assert.Equal(1, bob.SessionsPlayed);
    }

    [Fact]
    public async Task GetLeaderBoardWithRank_ReturnsSortedRankedList()
    {
        // add leaderboard entries with scores and times for ranking
        _context.LeaderBoardEntries.AddRange(new[]
        {
            new LeaderBoardEntry { PlayerId = 1, PlayerName = "Alice", TotalScore = 20, TotalTime = 1500, SessionsPlayed = 3 },
            new LeaderBoardEntry { PlayerId = 2, PlayerName = "Bob", TotalScore = 20, TotalTime = 1400, SessionsPlayed = 2 },
            new LeaderBoardEntry { PlayerId = 3, PlayerName = "Charlie", TotalScore = 15, TotalTime = 1200, SessionsPlayed = 1 }
        });

        await _context.SaveChangesAsync();

        // retrieve leaderboard entries ordered by score and time
        var ranked = await LeaderBoardService.GetLeaderBoardWithRank();

        // verify the order and ranks are correct
        Assert.Equal(3, ranked.Count);

        // Bob and Alice have the same score, but Bob has less total time, so Bob is ranked higher
        Assert.Equal("Bob", ranked[0].PlayerName);
        Assert.Equal(1, ranked[0].Rank);

        Assert.Equal("Alice", ranked[1].PlayerName);
        Assert.Equal(2, ranked[1].Rank);

        Assert.Equal("Charlie", ranked[2].PlayerName);
        Assert.Equal(3, ranked[2].Rank);
    }
}
