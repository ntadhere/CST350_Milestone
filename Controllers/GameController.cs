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
        private static GameCollection gameCollection = new GameCollection();

        public IActionResult Index()
        {
            int? boardSize = HttpContext.Session.GetInt32("BoardSize");
            int? difficulty = HttpContext.Session.GetInt32("Difficulty");

            if (boardSize.HasValue && difficulty.HasValue)
            {
                gameCollection.GenerateBoard(boardSize.Value);
                gameCollection.SetupLiveNeighbors(difficulty.Value);
                gameCollection.CalculateLiveNeighbors();

                // Start timer as soon as player starts game
                HttpContext.Session.SetObjectAsJson("StartTime", DateTime.Now);

                // Save only necessary game state
                HttpContext.Session.SetObjectAsJson("Board", gameCollection.Board);
                HttpContext.Session.SetObjectAsJson("GameStatus", "Game in Progress");

                return View("Index", gameCollection.Board);
            }

            return View("AccessDenied");
        }
        public IActionResult GameBoard()
        {
            return PartialView("GameBoard", gameCollection.Board);
        }
        // Action method to process right mouse clicks to place a flag
        [HttpPost]
        public IActionResult RightClickShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // Toggle flag state
            gameCollection.Board.TheGrid[row, col].IsFlag = !gameCollection.Board.TheGrid[row, col].IsFlag;

            // Save the updated board to session
            HttpContext.Session.SetObjectAsJson("Board", gameCollection.Board);
            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        }


        [HttpPost]
        public IActionResult LeftClickShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;
            if (gameCollection.Board.TheGrid[row, col].IsFlag)
            {
                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
            }
            else
            {
                // If the cell has no neighbors, perform FloodFill and return the entire board
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
                    
                    // Check if the player win by calling the checker from GameCollection 
                    if (gameCollection.IsWin() == true)
                    {
                        // Return a JSON response with the redirect URL for the WinPage
                        return Json(new { redirectUrl = Url.Action("WinPage") });
                    }

                    // Check if the cell is a bomb then return the lose page
                    else if (gameCollection.Board.TheGrid[row, col].IsLive == true)
                    {
                        foreach (var cell in gameCollection.Board.TheGrid)
                        {
                            cell.IsVisited = true;
                        }
                        // Return a JSON response with the redirect URL for the LosePage
                        return Json(new { redirectUrl = Url.Action("LosePage") });
                    }
                    // Return only the updated button for the clicked cell
                    return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
                }
            }
            
        }

    // Action method to process left mouse clicks
    public IActionResult ShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        }
        public IActionResult WinPage()
        {
            int elapsedTime = GetElapsedTime();
            int score = gameCollection.CalculateGameScore(elapsedTime);
            ViewBag.GameStatus = score;
            return View();
        }

        // lose section
        public IActionResult LosePage()
        {
            int elapsedTime = GetElapsedTime();
            int score = gameCollection.CalculateGameScore(elapsedTime);
            ViewBag.GameStatus = score;
            return View();
        }

        /// <summary>
        /// Calculate elapsed time
        /// </summary>
        /// <returns></returns>
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

        [HttpPost]
        public IActionResult SaveGame()
        {

            string boardJson = ServiceStack.Text.JsonSerializer.SerializeToString(gameCollection.Board);


            // Retrieve the game board and user information from the session
            //var gameBoard = HttpContext.Session.GetObjectFromJson<BoardModel>("Board");
            var user = HttpContext.Session.GetObjectFromJson<UserModel>("User"); // Assuming user is stored in session

            if (boardJson == null || user == null)
            {
                return BadRequest("Game state or user information is missing.");
            }

            // Serialize the game state and user info into JSON
            //string gameStateJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            //{
            //    Board = gameBoard,
            //    UserId = user.Id
            //});

            // Call a service method to save the JSON string into the database
            bool isSaved = gameCollection.SaveGameState(user.Id, boardJson);

            if (isSaved)
            {
                return Json(new { success = true, message = "Game state saved successfully!" });
            }

            return Json(new { success = false, message = "Failed to save game state." });
        }


        public IActionResult GetSavedGames()
        {
            return PartialView("GetSavedGames",gameCollection.GetAllSavedGame());
        }

        [HttpPost]
        public IActionResult DeleteGameById(int id)
        {
            try
            {
                if (gameCollection.DeleteGameById(id))
                {
                    return Json(new { success = true, message = "Game deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete the game." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public IActionResult LoadSavedGame(int id)
        {
            try
            {
                if (gameCollection.GetSavedGameById(id) == null)
                {
                    return Json(new { success = false, message = "Saved game not found." });
                }

                // Deserialize the GameData string into the BoardModel object
                var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<BoardModel>(gameCollection.GetSavedGameById(id).GameData);

                // Return the updated gameboard as a PartialView
                return PartialView("GameBoard", gameBoard);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


    }
}
