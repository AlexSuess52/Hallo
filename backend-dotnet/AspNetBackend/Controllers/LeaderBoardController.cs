using AspNetBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetBackend.Controllers 
{
    /// <summary>
    /// API controller for leaderboard operations.
    /// </summary>
    [Route("api/leaderboard")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private readonly ILeaderBoardService _leaderBoardService;

        public LeaderBoardController(ILeaderBoardService leaderBoardService)
        {
            _leaderBoardService = leaderBoardService ?? throw new ArgumentNullException(nameof(leaderBoardService));
        }

        /// <summary>
        /// Retrieves all quiz sessions.
        /// </summary>
        /// <returns>A list of all quiz sessions.</returns>
        [HttpGet("get-all-quiz-sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _leaderBoardService.GetAllQuizSessions();
            return Ok(sessions);
        }

        /// <summary>
        /// Recalculates the leaderboard and updates the database.
        /// </summary>
        /// <returns>A success message if the leaderboard was updated.</returns>
        [HttpPost("recalculate-leaderboard")]
        public async Task<IActionResult> RecalculateLeaderBoard()
        {
            await _leaderBoardService.RecalculateLeaderBoard();
            return Ok(new { message = "Leaderboard in Datenbank aktualisiert." });
        }

        /// <summary>
        /// Retrieves the current leaderboard including ranking information.
        /// </summary>
        /// <returns>The current leaderboard with ranks.</returns>
        [HttpGet("get-current-ranking")]
        public async Task<IActionResult> GetLeaderBoardWithRank()
        {
            var leaderboard = await _leaderBoardService.GetLeaderBoardWithRank();
            return Ok(leaderboard);
        }
    }
}

