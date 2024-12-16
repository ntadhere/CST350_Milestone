// Importing necessary namespaces
using CST350_Milestone.Filter;
using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using CST350_Milestone.Services.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CST350_Milestone.Controllers
{
    public class GameController : Controller
    {
        // Static instance of GameCollection to manage game state
        private static GameCollection gameCollection = new GameCollection();

        // Default action to initialize the game board
        public IActionResult Index()
        {
            // Retrieve game configuration from session (board size and difficulty)
            int? boardSize = HttpContext.Session.GetInt32("BoardSize");
            int? difficulty = HttpContext.Session.GetInt32("Difficulty");

            // If configuration exists, initialize the game board
            if (boardSize.HasValue && difficulty.HasValue)
            {
                // Generate and set up the game board
                gameCollection.GenerateBoard(boardSize.Value);
                gameCollection.SetupLiveNeighbors(difficulty.Value);
                gameCollection.CalculateLiveNeighbors();

                // Save the start time in the session
                HttpContext.Session.SetObjectAsJson("StartTime", DateTime.Now);

                // Save the initial game state to the session
                HttpContext.Session.SetObjectAsJson("Board", gameCollection.Board);
                HttpContext.Session.SetObjectAsJson("GameStatus", "Game in Progress");

                // Render the game board view
                return View("Index", gameCollection.Board);
            }

            // Redirect to "Access Denied" if configuration is missing
            return View("AccessDenied");
        }

        // Partial view to render the game board
        public IActionResult GameBoard()
        {
            return PartialView("GameBoard", gameCollection.Board);
        }

        // Handle right-click actions (place/remove a flag on a cell)
        [HttpPost]
        public IActionResult RightClickShowOneButton(int cellNumber)
        {
            // Calculate cell's row and column based on its number
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // Toggle the flag state of the cell
            gameCollection.Board.TheGrid[row, col].IsFlag = !gameCollection.Board.TheGrid[row, col].IsFlag;

            // Save the updated board state to the session
            HttpContext.Session.SetObjectAsJson("Board", gameCollection.Board);

            // Return a partial view with the updated cell
            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        }

        // Handle left-click actions (reveal a cell or process a game event)
        [HttpPost]
        public IActionResult LeftClickShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // If the cell is flagged, return without any action
            if (gameCollection.Board.TheGrid[row, col].IsFlag)
            {
                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
            }
            else
            {
                // If the cell has no neighbors, use FloodFill to reveal adjacent cells
                if (gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
                {
                    gameCollection.FloodFill(row, col);
                    gameCollection.Board.TheGrid[row, col].IsVisited = true;

                    // Return the updated game board
                    return PartialView("GameBoard", gameCollection.Board);
                }
                else
                {
                    // Mark the cell as visited
                    gameCollection.Board.TheGrid[row, col].IsVisited = true;

                    // Check for win condition
                    if (gameCollection.IsWin())
                    {
                        return Json(new { redirectUrl = Url.Action("WinPage") });
                    }

                    // Check for lose condition (cell is a bomb)
                    else if (gameCollection.Board.TheGrid[row, col].IsLive)
                    {
                        // Reveal all cells
                        foreach (var cell in gameCollection.Board.TheGrid)
                        {
                            cell.IsVisited = true;
                        }

                        return Json(new { redirectUrl = Url.Action("LosePage") });
                    }

                    // Return a partial view with the updated cell
                    return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
                }
            }
        }

        // Render a single button (cell) view
        public IActionResult ShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        }

        // Render the win page
        public IActionResult WinPage()
        {
            int elapsedTime = GetElapsedTime();
            int score = gameCollection.CalculateGameScore(elapsedTime);
            ViewBag.GameStatus = score;
            return View();
        }

        // Render the lose page
        public IActionResult LosePage()
        {
            int elapsedTime = GetElapsedTime();
            int score = gameCollection.CalculateGameScore(elapsedTime);
            ViewBag.GameStatus = score;
            return View();
        }

        // Calculate the elapsed time from the start of the game
        public int GetElapsedTime()
        {
            DateTime? startTime = HttpContext.Session.GetObjectFromJson<DateTime?>("StartTime");
            if (startTime.HasValue)
            {
                TimeSpan elapsed = DateTime.Now - startTime.Value;
                return (int)elapsed.TotalSeconds;
            }
            return 0;
        }

        // Save the current game state
        [HttpPost]
        public IActionResult SaveGame()
        {
            string boardJson = ServiceStack.Text.JsonSerializer.SerializeToString(gameCollection.Board);
            var user = HttpContext.Session.GetObjectFromJson<UserModel>("User");

            if (boardJson == null || user == null)
            {
                return BadRequest("Game state or user information is missing.");
            }

            // Save the game state to the database
            bool isSaved = gameCollection.SaveGameState(user.Id, boardJson);

            return Json(new { success = isSaved, message = isSaved ? "Game state saved successfully!" : "Failed to save game state." });
        }

        // Retrieve all saved games for the user
        public IActionResult GetSavedGames()
        {
            return PartialView("GetSavedGames", gameCollection.GetAllSavedGame());
        }

        // Delete a saved game by its ID
        [HttpPost]
        public IActionResult DeleteGameById(int id)
        {
            try
            {
                bool success = gameCollection.DeleteGameById(id);
                return Json(new { success, message = success ? "Game deleted successfully." : "Failed to delete the game." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // Load a saved game by its ID
        public IActionResult LoadSavedGame(int id)
        {
            try
            {
                var savedGame = gameCollection.GetSavedGameById(id);
                if (savedGame == null)
                {
                    return Json(new { success = false, message = "Saved game not found." });
                }

                var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<BoardModel>(savedGame.GameData);
                return PartialView("GameBoard", gameBoard);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
