using AspNetBackend.Dtos;

namespace AspNetBackend.Services;

public interface ILeaderBoardService
{
    Task<List<QuizSessionDto>> GetAllQuizSessions();
    Task<List<PlayerScoreWithRankDto>> GetLeaderBoardWithRank();
    Task RecalculateLeaderBoard();
}
