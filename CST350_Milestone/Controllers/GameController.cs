using CST350_Milestone.Filter;
using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
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
                BoardModel board = gameCollection.GenerateBoard(boardSize.Value);
                gameCollection.SetupLiveNeighbors(difficulty.Value);
                gameCollection.CalculateLiveNeighbors();

                // Start timer as soon as player starts game
                HttpContext.Session.SetObjectAsJson("StartTime", DateTime.Now);

                // Save only necessary game state
                HttpContext.Session.SetObjectAsJson("Board", board);
                HttpContext.Session.SetObjectAsJson("GameStatus", "Game in Progress");

                return View("Index", board);
            }

            return View("AccessDenied");
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

        // Action method to process left mouse clicks
        public IActionResult ShowOneButton(int cellNumber)
        {
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // If flagged, do nothing
            if (gameCollection.Board.TheGrid[row, col].IsFlag)
            {
                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
            }

            // If it's a bomb, reveal the whole board
            if (gameCollection.Board.TheGrid[row, col].IsLive)
            {
                foreach (var cell in gameCollection.Board.TheGrid)
                {
                    cell.IsVisited = true;
                }

                // Set Game Status
                HttpContext.Session.SetString("GameStatus", "You Lose");
                ViewBag.GameStatus = "You lose!";
                return RedirectToAction("LosePage");
            }
            else
            {
                // If no bomb, update cell and check for win
                if (gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
                {
                    gameCollection.FloodFill(row, col);
                }

                gameCollection.Board.TheGrid[row, col].IsVisited = true;

                if (gameCollection.IsWin())
                {
                    HttpContext.Session.SetString("GameStatus", "You Win");
                    ViewBag.GameStatus = "You win!";
                    return RedirectToAction("WinPage");
                }
            }

            // Save updated board state
            HttpContext.Session.SetObjectAsJson("Board", gameCollection.Board);
            ViewBag.GameStatus = HttpContext.Session.GetString("GameStatus") ?? "Game in Progress";

            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        }

        public IActionResult HandleButtonClick(int cellNumber)
        {
            //var gameCollection = HttpContext.Session.GetObjectFromJson<GameCollection>("GameCollection") ?? new GameCollection();

            // Calculate row and column based on cellNumber
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;


            // Check if the clicked cell is a bomb
            if (gameCollection.Board.TheGrid[row, col].IsLive)
            {
                // Reveal the entire board
                foreach (var cell in gameCollection.Board.TheGrid)
                {
                    cell.IsVisited = true;
                }
                // Pass a flag to the view to show "You Lose" message
                HttpContext.Session.SetString("GameStatus", "You Lose");
                ViewBag.GameStatus = LosePage();
                return RedirectToAction("LosePage");
            }
            else
            {
                // FloodFill for 0
                // If clicked cell has no neighboring boms, trigger flood fill
                if (gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
                {
                    gameCollection.FloodFill(row, col);
                }

                // If it's not a bomb, set this cell to visited
                gameCollection.Board.TheGrid[row, col].IsVisited = true;

                // Check for win condition after this click
                if (gameCollection.IsWin())
                {
                    HttpContext.Session.SetString("GameStatus", "You Win");
                    ViewBag.GameStatus = WinPage();
                    return RedirectToAction("WinPage");
                }
            }

            // Save the updated game state to the session
            HttpContext.Session.SetObjectAsJson("GameCollection", gameCollection);
            ViewBag.GameStatus = HttpContext.Session.GetString("GameStatus") ?? "Game in Progress";
            // Pass the gameCollection back to the view
            return View("Index", gameCollection.Board);
        }

        // win section
        public IActionResult WinPage()
        {
            // calculate socre
            int elapsedTime = GetElapsedTime();
            // call method from game collection class
            int score = gameCollection.CalculateGameScore(elapsedTime);
            // pass score to win page
            ViewBag.GameStatus = score;
            return View();
        }

        // lose section
        public IActionResult LosePage()
        {
            // calculate socre
            int elapsedTime = GetElapsedTime();
            // call method from game collection class
            int score = gameCollection.CalculateGameScore(elapsedTime);
            // pass score to lose page
            ViewBag.GameStatus = score;
            return View();
        }

        /// <summary>
        /// Calculate elapsed time
        /// </summary>
        /// <returns></returns>
        public int GetElapsedTime()
        {
            // Retrieve the start time from session as a DateTime object
            DateTime? startTime = HttpContext.Session.GetObjectFromJson<DateTime?>("StartTime");
            if (startTime.HasValue)
            {
                // Calculate the difference in seconds
                TimeSpan elapsed = DateTime.Now - startTime.Value;
                return (int)elapsed.TotalSeconds;
            }
            return 0;
        }
    }
}
