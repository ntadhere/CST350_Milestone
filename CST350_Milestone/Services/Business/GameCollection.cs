using CST350_Milestone.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System;

namespace CST350_Milestone.Services.Business
{
    public class GameCollection
    {
        private BoardModel board = new BoardModel();

        // Public getter for the board to expose it for read-only access
        public BoardModel Board => board;

        /// <summary>
        /// Generates a new game board with the specified size and initializes the grid.
        /// </summary>
        /// <param name="size">The size of the board</param>
        /// <returns>Returns the populated board model</returns>
        public BoardModel GenerateBoard(int size)
        {
            int id = 0;
            board.Size = size;
            board.TheGrid = new CellModel[size, size];
            board.CellListData = new List<CellModel>();

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
        /// Checks if a square is within the boundaries of the board.
        /// </summary>
        /// <param name="col">Column index</param>
        /// <param name="row">Row index</param>
        /// <returns>True if the square is within the board; otherwise false</returns>
        public bool IsSquareOnBoard(int col, int row)
        {
            return row >= 0 && row < board.Size && col >= 0 && col < board.Size;
        }

        /// <summary>
        /// Checks whether the game has been won.
        /// </summary>
        /// <returns>True if the game is won; otherwise false</returns>
        public bool IsWin()
        {
            int nonRevealedCells = 0;
            foreach (var cell in board.CellListData)
            {
                if (!cell.IsVisited)
                    nonRevealedCells++;
            }

            return nonRevealedCells == board.Difficulty;
        }

        /// <summary>
        /// Initializes live bomb cells randomly across the board based on the difficulty.
        /// </summary>
        /// <param name="difficulty">The difficulty determining the number of bombs</param>
        public void SetupLiveNeighbors(int difficulty)
        {
            board.Difficulty = difficulty;
            Random rnd = new Random();

            for (int x = 0; x < board.Difficulty; x++)
            {
                bool isSuccess = false;
                while (!isSuccess)
                {
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
        /// Calculates the number of live neighbors for each cell on the grid.
        /// </summary>
        public void CalculateLiveNeighbors()
        {
            // Iterate through all rows of the board
            for (int i = 0; i < board.Size; i++)
            {
                // Iterate through all columns of the board
                for (int j = 0; j < board.Size; j++)
                {
                    // Get the current cell at position (i, j)
                    var currentCell = board.TheGrid[i, j];

                    // If the current cell is alive (a bomb), it has 9 neighbors
                    if (currentCell.IsLive)
                    {
                        currentCell.NumNeighbors = 9; // Bombs have 9 neighbors
                    }
                    else
                    {
                        // For non-live cells, check the 8 surrounding cells (neighbors)
                        for (int r = -1; r <= 1; r++)
                        {
                            for (int c = -1; c <= 1; c++)
                            {
                                // Check if the neighbor is within the bounds of the grid
                                if (IsSquareOnBoard(currentCell.ColNumber + r, currentCell.RowNumber + c) &&
                                    // Check if the neighboring cell is alive
                                    board.TheGrid[currentCell.ColNumber + r, currentCell.RowNumber + c].IsLive)
                                {
                                    // Increment the number of neighbors if the adjacent cell is alive
                                    currentCell.NumNeighbors++;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies flood fill to a given cell to reveal adjacent cells with 0 neighbors.
        /// </summary>
        /// <param name="col">Column index of the start cell</param>
        /// <param name="row">Row index of the start cell</param>
        public void FloodFill(int col, int row)
        {
            if (!IsSquareOnBoard(col, row)) return;
            var currentCell = board.TheGrid[col, row];

            if (currentCell.NumNeighbors != 0 || currentCell.IsVisited) return;

            currentCell.IsVisited = true;
            FloodFill(col, row - 1); // North
            FloodFill(col + 1, row); // East
            FloodFill(col, row + 1); // South
            FloodFill(col - 1, row); // West
        }

        /// <summary>
        /// Calculates the final score based on elapsed time.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time during the game</param>
        /// <returns>The calculated score</returns>
        public int CalculateGameScore(int elapsedTime)
        {
            return Math.Max(elapsedTime, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="boardSize"></param>
        /// <returns></returns>
        public int ScoreCalculator(int difficulty, int boardSize)
        {
            int score = (boardSize * difficulty);
            return Math.Max(score, 0);
        }


    }
}
