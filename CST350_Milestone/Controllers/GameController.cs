using CST350_Milestone.Filter;
using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing;
using System.Text.Json;

namespace CST350_Milestone.Controllers
{

    public class GameController : Controller
    {
        static GameCollection gameCollection = new GameCollection();

        public IActionResult Index()
        {
            // Retrieve the "BoardSize" and "Difficulty" values from the session and assign them to nullable integer variables.
            // If these values are not present in the session, the variables will be set to null.
            int? boardSize = HttpContext.Session.GetInt32("BoardSize");
            int? difficulty = HttpContext.Session.GetInt32("Difficulty");


            if (boardSize.HasValue && difficulty.HasValue)
            {
                // Initialize the board with the retrieved size
                BoardModel board = gameCollection.GenerateBoard(boardSize.Value);

                // Set up live neighbors based on the retrieved difficulty
                gameCollection.SetupLiveNeighbors(difficulty.Value);
                gameCollection.CalculateLiveNeighbors();

                // Start timer as soon as player starts game
                HttpContext.Session.SetObjectAsJson("StartTime", DateTime.Now);

                HttpContext.Session.SetObjectAsJson("GameCollection", gameCollection);


                return View("Index", board);
            }
            return View("AccessDenied");
        }

        // Action method to process right mouse clicks to place a flag
        [HttpPost]
        public IActionResult RightClickShowOneButton(int cellNumber)
        {
            // Calculate row and column based on cellNumber
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // If there already a flag there, remove it then return the update button
            if (gameCollection.Board.TheGrid[row, col].IsFlag == true)
            {
                gameCollection.Board.TheGrid[row, col].IsFlag = false;
            }
            // If ther is no flag there, plant a flag
            else
            {
                gameCollection.Board.TheGrid[row, col].IsFlag = true;
            }

            return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
            //return View("Index", buttons);

        }

        ////Action method to process left mouse clicks
        ////If the cell has flag, the cell will not response to the left-click events
        //// If the cell does not has flag, we will show the number of neighbors
        //public IActionResult ShowOneButton(int cellNumber)
        //{
        //    // Calculate row and column based on cellNumber
        //    int row = cellNumber / gameCollection.Board.Size;
        //    int col = cellNumber % gameCollection.Board.Size;

        //    // check if there is a flag already in the cell, the cell will not response to the left-click events
        //    if (gameCollection.Board.TheGrid[row, col].IsFlag == true)
        //    {
        //        return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        //    }
        //    else
        //    {
        //        // Check if the clicked cell is a bomb
        //        if (gameCollection.Board.TheGrid[row, col].IsLive)
        //        {
        //            // Reveal the entire board
        //            foreach (var cell in gameCollection.Board.TheGrid)
        //            {
        //                cell.IsVisited = true;
        //            }
        //            // Pass a flag to the view to show "You Lose" message
        //            HttpContext.Session.SetString("GameStatus", "You Lose");
        //            ViewBag.GameStatus = "You lose!";
        //            return RedirectToAction("LosePage");
        //        }
        //        else
        //        {
        //            // FloodFill for 0
        //            // If clicked cell has no neighboring boms, trigger flood fill
        //            if (gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
        //            {
        //                gameCollection.FloodFill(row, col);

        //                // If it's not a bomb, set this cell to visited
        //                gameCollection.Board.TheGrid[row, col].IsVisited = true;

        //                return View("Index", gameCollection.Board);
        //            }



        //            // Check for win condition after this click
        //            if (gameCollection.IsWin())
        //            {
        //                HttpContext.Session.SetString("GameStatus", "You Win");
        //                ViewBag.GameStatus = "You win!";
        //                return RedirectToAction("WinPage");
        //            }

        //            else
        //            {
        //                // Pass the gameCollection back to the view
        //                //return View("Index", gameCollection.Board);
        //                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
        //            }
        //        }

        //    }

        //    // Save the updated game state to the session
        //    HttpContext.Session.SetObjectAsJson("GameCollection", gameCollection);
        //    ViewBag.GameStatus = HttpContext.Session.GetString("GameStatus") ?? "Game in Progress";

        //}

        //Action method to process left mouse clicks
        //If the cell has flag, the cell will not response to the left-click events
        // If the cell does not has flag, we will show the number of neighbors
        public IActionResult ShowOneButton(int cellNumber)
        {
            // Calculate row and column based on cellNumber
            int row = cellNumber / gameCollection.Board.Size;
            int col = cellNumber % gameCollection.Board.Size;

            // check if there is a flag already in the cell, the cell will not response to the left-click events
            if (gameCollection.Board.TheGrid[row, col].IsFlag == true)
            {
                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
            }

            else if (gameCollection.Board.TheGrid[row, col].IsLive)
            {
                // Reveal the entire board
                foreach (var cell in gameCollection.Board.TheGrid)
                {
                    cell.IsVisited = true;
                }
                // Pass a flag to the view to show "You Lose" message
                HttpContext.Session.SetString("GameStatus", "You Lose");
                ViewBag.GameStatus = "You lose!";
                return RedirectToAction("LosePage");
            }
            else if (gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
            {
                gameCollection.FloodFill(row, col);
                Thread.Sleep(1000);

                // If it's not a bomb, set this cell to visited
                gameCollection.Board.TheGrid[row, col].IsVisited = true;

                return View("Index", gameCollection.Board);

            }
            else
            {
                return PartialView("ShowOneButton", gameCollection.Board.TheGrid[row, col]);
                //return RedirectToAction("LosePage");


            }

            //    // Check for win condition after this click
            //    if (gameCollection.IsWin())
            //    {
            //        HttpContext.Session.SetString("GameStatus", "You Win");
            //        ViewBag.GameStatus = "You win!";
            //        return RedirectToAction("WinPage");
            //    }

            //}

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
                ViewBag.GameStatus = "You lose!";
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
                    ViewBag.GameStatus = "You win!";
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
            // calculate score
            int elapsedTime = GetElapsedTime();
            int boardSize = HttpContext.Session.GetInt32("BoardSize").Value;
            int difficulty = HttpContext.Session.GetInt32("Difficulty").Value;

            // call method from game collecgion class
            int score = gameCollection.gameCalculation(elapsedTime, boardSize, difficulty);

            // pass score to win page
            ViewBag.Score = score;

            return View();
        }

        // lose section
        public IActionResult LosePage()
        {
            // calculate socre
            int elapsedTime = GetElapsedTime();
            int boardSize = HttpContext.Session.GetInt32("BoardSize").Value;
            int difficulty = HttpContext.Session.GetInt32("Difficulty").Value;

            // call method from game collecgion class
            int score = gameCollection.gameCalculation(elapsedTime, boardSize, difficulty);

            // pass score to win page
            ViewBag.Score = score;
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
