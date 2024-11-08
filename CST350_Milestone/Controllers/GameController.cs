using CST350_Milestone.Filter;
using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CST350_Milestone.Controllers
{

    public class GameController : Controller
    {
        static GameCollection gameCollection = new GameCollection();

        public IActionResult Index()
        {
            int? boardSize = HttpContext.Session.GetInt32("BoardSize");
            int? difficulty = HttpContext.Session.GetInt32("Difficulty");

            if (boardSize.HasValue && difficulty.HasValue)
            {
                // **EDITED THIS PART!!!
                BoardModel board = gameCollection.GenerateBoard(boardSize.Value);
                //BoardModel board = new BoardModel();
                // Initialize the board with the retrieved size
                //board = gameCollection.GenerateBoard(boardSize.Value);

                // Set up live neighbors based on the retrieved difficulty
                gameCollection.SetupLiveNeighbors(difficulty.Value);
                gameCollection.CalculateLiveNeighbors();

                HttpContext.Session.SetObjectAsJson("GameCollection", gameCollection);


                return View("Index", board);
            }
            return View("AccessDenied");
        }
        public IActionResult HandleButtonClick(int buttonNumber)
        {
            //var gameCollection = HttpContext.Session.GetObjectFromJson<GameCollection>("GameCollection") ?? new GameCollection();

            // Calculate row and column based on buttonNumber
            int row = buttonNumber / gameCollection.Board.Size;
            int col = buttonNumber % gameCollection.Board.Size;


            // Check if the clicked cell is a bomb
            if (gameCollection.Board.TheGrid[row, col].IsLive)
            {
                // Reveal the entire board
                foreach (var cell in gameCollection.Board.TheGrid)
                {
                    cell.IsVisited = true;
                }
                // Pass a flag to the view to show "You Lose" message
                // **EDITED PART!!
                return RedirectToAction("LosePage");
            }
            else
            {
                // If it's not a bomb, set this cell to visited
                gameCollection.Board.TheGrid[row, col].IsVisited = true;

                // Check for win condition after this click
                if (gameCollection.IsWin())
                {
                    // ** EDITED PART!!
                    return RedirectToAction("WinPage");
                }
            }

            // Save the updated game state to the session
            HttpContext.Session.SetObjectAsJson("GameCollection", gameCollection);

            // Pass the gameCollection back to the view
            return View("Index", gameCollection.Board);
        }
        // **Added PARTS!!!
        // win section
        public IActionResult WinPage()
        {
            return View();
        }

        // lose section
        public IActionResult LosePage()
        {
            return View();
        }

    }
}
