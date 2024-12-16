using CST350_Milestone.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System;
using CST350_Milestone.Services.DataAccess;

namespace CST350_Milestone.Services.Business
{
    public class GameCollection
    {
        // Represents the game board model instance
        private BoardModel board = new BoardModel();

        // Public getter for the board to provide read-only access
        public BoardModel Board => board;

        /// <summary>
        /// Generates a new game board with the specified size and initializes its cells.
        /// </summary>
        /// <param name="size">Size of the board (number of rows and columns)</param>
        /// <returns>The initialized board model</returns>
        public BoardModel GenerateBoard(int size)
        {
            int id = 0; // Unique ID for each cell
            board.Size = size;
            board.TheGrid = new CellModel[size, size];
            board.CellListData = new List<CellModel>();

            // Initialize each cell in the grid
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board.TheGrid[i, j] = new CellModel(i, j);
                    board.CellListData.Add(board.TheGrid[i, j]);
                    board.TheGrid[i, j].Id = id;
                    id++;
                }
            }
            return board;
        }

        /// <summary>
        /// Checks if a given cell is within the boundaries of the board.
        /// </summary>
        /// <param name="col">Column index</param>
        /// <param name="row">Row index</param>
        /// <returns>True if within bounds, otherwise false</returns>
        public bool IsSquareOnBoard(int col, int row)
        {
            return row >= 0 && row < board.Size && col >= 0 && col < board.Size;
        }

        /// <summary>
        /// Determines if the game has been won by checking non-revealed cells.
        /// </summary>
        /// <returns>True if all non-bomb cells are revealed, otherwise false</returns>
        public bool IsWin()
        {
            int nonRevealedCells = 0;
            foreach (var cell in board.CellListData)
            {
                if (!cell.IsVisited)
                    nonRevealedCells++;
            }

            // Game is won if non-revealed cells match the number of bombs
            return nonRevealedCells == board.Difficulty;
        }

        /// <summary>
        /// Sets up live bomb cells randomly across the board based on difficulty level.
        /// </summary>
        /// <param name="difficulty">Number of bombs to place</param>
        public void SetupLiveNeighbors(int difficulty)
        {
            board.Difficulty = difficulty;
            Random rnd = new Random();

            for (int x = 0; x < board.Difficulty; x++)
            {
                bool isSuccess = false;
                while (!isSuccess)
                {
                    // Randomly select a cell for a bomb
                    int newRow = rnd.Next(0, board.Size);
                    int newCol = rnd.Next(0, board.Size);
                    if (!board.TheGrid[newCol, newRow].IsLive)
                    {
                        board.TheGrid[newCol, newRow].IsLive = true;
                        isSuccess = true;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the number of live neighbors for each cell.
        /// </summary>
        public void CalculateLiveNeighbors()
        {
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    var currentCell = board.TheGrid[i, j];

                    if (currentCell.IsLive)
                    {
                        // Bomb cells are marked with a special neighbor count
                        currentCell.NumNeighbors = 9;
                    }
                    else
                    {
                        // Count neighboring bombs
                        for (int r = -1; r <= 1; r++)
                        {
                            for (int c = -1; c <= 1; c++)
                            {
                                if (IsSquareOnBoard(currentCell.ColNumber + r, currentCell.RowNumber + c) &&
                                    board.TheGrid[currentCell.ColNumber + r, currentCell.RowNumber + c].IsLive)
                                {
                                    currentCell.NumNeighbors++;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Implements a flood-fill algorithm to reveal adjacent empty cells.
        /// </summary>
        /// <param name="col">Column index of the starting cell</param>
        /// <param name="row">Row index of the starting cell</param>
        public void FloodFill(int col, int row)
        {
            if (!IsSquareOnBoard(col, row)) return;
            var currentCell = board.TheGrid[col, row];

            // Stop if the cell is already visited or not empty
            if (currentCell.NumNeighbors != 0 || currentCell.IsVisited) return;

            currentCell.IsVisited = true;

            // Recursively reveal adjacent cells
            FloodFill(col, row - 1); // North
            FloodFill(col + 1, row); // East
            FloodFill(col, row + 1); // South
            FloodFill(col - 1, row); // West
        }

        /// <summary>
        /// Calculates the player's score based on elapsed time and difficulty.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed during the game</param>
        /// <returns>The calculated score</returns>
        public int CalculateGameScore(int elapsedTime)
        {
            return Math.Max(elapsedTime, 0);
        }

        /// <summary>
        /// Saves the current game state to the database.
        /// </summary>
        /// <param name="userId">User ID of the player</param>
        /// <param name="gameDataJson">Serialized game data</param>
        /// <returns>True if save was successful, otherwise false</returns>
        public bool SaveGameState(int userId, string gameDataJson)
        {
            UserDAO userDao = new UserDAO();
            return userDao.SaveGameState(userId, gameDataJson);
        }

        /// <summary>
        /// Retrieves all saved games from the database.
        /// </summary>
        /// <returns>A list of saved game models</returns>
        public List<SavedGameModel> GetAllSavedGame()
        {
            UserDAO userDAO = new UserDAO();
            return userDAO.GetAllSavedGames();
        }

        /// <summary>
        /// Deletes a saved game from the database by ID.
        /// </summary>
        /// <param name="id">ID of the saved game to delete</param>
        /// <returns>True if deletion was successful, otherwise false</returns>
        public bool DeleteGameById(int id)
        {
            UserDAO userDAO = new UserDAO();
            return userDAO.DeleteGameById(id);
        }

        /// <summary>
        /// Retrieves a specific saved game by its ID.
        /// </summary>
        /// <param name="id">ID of the saved game</param>
        /// <returns>The retrieved saved game model</returns>
        public SavedGameModel GetSavedGameById(int id)
        {
            UserDAO userDAO = new UserDAO();
            return userDAO.GetSavedGameById(id);
        }
    }
}
