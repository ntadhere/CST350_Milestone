using CST350_Milestone.Models;
using System;

namespace CST350_Milestone.Services.Business
{
    public class GameService
    {
        private GameCollection _gameCollection;

        public GameService()
        {
            _gameCollection = new GameCollection();
        }

        // Initialize the board and set up game settings
        public BoardModel StartNewGame(int boardSize, int difficulty)
        {
            var board = _gameCollection.GenerateBoard(boardSize);
            _gameCollection.SetupLiveNeighbors(difficulty);
            _gameCollection.CalculateLiveNeighbors();
            return board;
        }

        // Handle left-click action on a cell
        public GameResult HandleLeftClick(int cellNumber)
        {
            int row = cellNumber / _gameCollection.Board.Size;
            int col = cellNumber % _gameCollection.Board.Size;

            // Check if the cell is flagged (cannot click on flagged cells)
            if (_gameCollection.Board.TheGrid[row, col].IsFlag)
            {
                return new GameResult(_gameCollection.Board, "Cell is flagged.");
            }

            // Handle bomb hit (game over)
            if (_gameCollection.Board.TheGrid[row, col].IsLive)
            {
                // Reveal all cells (for simplicity, could be expanded with proper game end behavior)
                foreach (var cell in _gameCollection.Board.TheGrid)
                {
                    cell.IsVisited = true;
                }
                return new GameResult(_gameCollection.Board, "You Lose");
            }
            else
            {
                // If no bombs, reveal the cell
                if (_gameCollection.Board.TheGrid[row, col].NumNeighbors == 0)
                {
                    _gameCollection.FloodFill(row, col);
                }

                _gameCollection.Board.TheGrid[row, col].IsVisited = true;

                // Check if the game is won
                if (_gameCollection.IsWin())
                {
                    return new GameResult(_gameCollection.Board, "You Win");
                }
            }

            return new GameResult(_gameCollection.Board, "Game in Progress");
        }

        // Handle right-click action to toggle flags
        public GameResult HandleRightClick(int cellNumber)
        {
            // Calculate the row and column based on the cell number
            int row = cellNumber / _gameCollection.Board.Size;
            int col = cellNumber % _gameCollection.Board.Size;

            // Initialize the message variable to indicate flag status
            string message = "";
            // Check if the cell is already flagged
            if (_gameCollection.Board.TheGrid[row, col].IsFlag)
            {
                // If the cell is flagged, remove the flag and update the message
                _gameCollection.Board.TheGrid[row, col].IsFlag = false;
                message = "Flag removed.";
            }
            else
            {
                // If the cell is not flagged, place a flag and update the message
                _gameCollection.Board.TheGrid[row, col].IsFlag = true;
                message = "Flag placed.";
            }
            // Return the current game state and the message indicating the flag change
            return new GameResult(_gameCollection.Board, message);
        }

        // Calculate the elapsed time from the session start
        public int GetElapsedTime(DateTime startTime)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            return (int)elapsed.TotalSeconds;
        }
    }

    // GameResult class to encapsulate board and message
    public class GameResult
    {
        public BoardModel Board { get; set; }
        public string Message { get; set; }

        public GameResult(BoardModel board, string message)
        {
            Board = board;
            Message = message;
        }
    }
}
