using CST350_Milestone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CST350_Milestone.Controllers
{
    public class GameController : Controller
    {

        public IActionResult Index()
        {
            // Retrieve the session GameSessionId
            var sessionGameSessionId = HttpContext.Session.GetString("GameSessionId");

            // If there is no session game ID, the user hasn't started a game, so deny access
            if (string.IsNullOrEmpty(sessionGameSessionId))
            {
                // Redirect to an error or access denied page
                return View("AccessDenied");
            }

            int? boardSize = HttpContext.Session.GetInt32("BoardSize");
            int? difficulty = HttpContext.Session.GetInt32("Difficulty");

            if (boardSize.HasValue && difficulty.HasValue)
            {
                // Initialize the board with the retrieved size
                BoardModel board = new BoardModel(boardSize.Value);

                // Set up live neighbors based on the retrieved difficulty
                board.setupLiveNeighbors(difficulty.Value);

                return View("Index", board);
            }
            return View("AccessDenied");
        }
    }
}
