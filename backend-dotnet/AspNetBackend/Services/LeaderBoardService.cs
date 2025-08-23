using AspNetBackend.Data;
using AspNetBackend.Dtos;
using AspNetBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetBackend.Services;

/// <summary>
/// Service providing leaderboard management functionality.
/// </summary>
public class LeaderBoardsService : ILeaderBoardService
{
    private readonly AspNetPostgresDbContext _context;

    public LeaderBoardsService(AspNetPostgresDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all quiz sessions from the database.
    /// </summary>
    /// <returns>A list of quiz sessions as DTOs.</returns>
    public async Task<List<QuizSessionDto>> GetAllQuizSessions()
    {
        return await _context.QuizSessions
            .Select(q => new QuizSessionDto
            {
                Id = q.Id,
                TotalScore = q.TotalScore,
                TotalTime = q.TotalTime,
                PlayerId = q.PlayerId,
                QuizId = q.QuizId,
            })
            .ToListAsync();
    }

    /// <summary>
    /// Recalculates the leaderboard by aggregating quiz session data and updates the database.
    /// </summary>
    /// <remarks>
    /// This method deletes all existing leaderboard entries and replaces them with updated statistics.
    /// Uses a transaction to ensure data consistency.
    /// </remarks>
    public async Task RecalculateLeaderBoard()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var leaderboardData = await _context.QuizSessions
                .Join(_context.Players, session => session.PlayerId, player => player.Id, (session, player) => new { session, player })
                .GroupBy(x => new { x.player.Id, x.player.Name })
                .Select(g => new LeaderBoardEntry
                {
                    PlayerId = g.Key.Id,
                    PlayerName = g.Key.Name,
                    TotalScore = g.Sum(x => x.session.TotalScore),
                    TotalTime = g.Sum(x => x.session.TotalTime),
                    SessionsPlayed = g.Count()
                })
                .ToListAsync();

            // deletes existing leaderboard entries
            _context.LeaderBoardEntries.RemoveRange(_context.LeaderBoardEntries);
            await _context.SaveChangesAsync();

            // inserts updated leaderboard data
            await _context.LeaderBoardEntries.AddRangeAsync(leaderboardData);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Retrieves the current leaderboard with player rankings.
    /// </summary>
    /// <returns>A ranked list of players including score, time, and sessions played.</returns>
    public async Task<List<PlayerScoreWithRankDto>> GetLeaderBoardWithRank()
    {
        // players are ranked by score, then by total time 
        // (faster players rank higher in case of a tie)
        var leaderboard = await _context.LeaderBoardEntries
            .OrderByDescending(e => e.TotalScore)
            .ThenBy(e => e.TotalTime)
            .ToListAsync();

        return leaderboard
            .Select((entry, index) => new PlayerScoreWithRankDto
            {
                Rank = index + 1,
                PlayerId = entry.PlayerId,
                PlayerName = entry.PlayerName,
                TotalScore = entry.TotalScore,
                TotalTime = entry.TotalTime,
                SessionsPlayed = entry.SessionsPlayed
            })
            .ToList();
    }
}
