using CST350_Milestone.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace CST350_Milestone.Services.Business
{
    public class GameCollection
    {
        // This is an in-memory list of users. Later this will be a db connection.
        // The _ prefix indicates a private field
        private BoardModel board = new BoardModel();

        public BoardModel Board => board; //Public getter to expose BoardModel

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    int maxDifficulty = Size * Size - 1;

        //    if (Difficulty < 0 || Difficulty > maxDifficulty)
        //    {
        //        yield return new ValidationResult(
        //            $"Difficulty must be between 0 and {maxDifficulty}.",
        //            new[] { nameof(Difficulty) });
        //    }
        //}

        public void generateBoard (int size)
        {
            board.Size = size;
            // we must initialize the array to avoid Null Exception errors
            board.TheGrid = new CellModel[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board.TheGrid[i, j] = new CellModel(i, j);
                }
            }
        }

        /// <summary>
        /// check if the square on the board or not
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool isSquareOnBoard(int col, int row)
        {
            // -9, 5 is out of bound
            if (row < 0 || row > board.Size - 1 || col < 0 || col > board.Size - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int getSquareOnBoard(int order)
        {
            int temp = 0;
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (temp == order)
                    {
                        return board.TheGrid[i, j].NumNeighbors;
                    }
                    temp++;
                }
            }
            return -1;
        }

        /// <summary>
        /// this method helps to check the win condition for the game.
        /// </summary>
        /// <returns></returns>
        public Boolean WinCondition()
        {
            int nonRevealCell = 0;
            Boolean isWin = false;

            // find the number of non-revealed cell
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.TheGrid[i, j].IsVisited == false)
                    {
                        nonRevealCell++;
                    }
                }
            }
            // if the number of bomb placed equal to the number of non-reveal cell
            if (board.Difficulty == nonRevealCell)
            {
                isWin = true;
            }
            return isWin;
        }

        /// <summary>
        /// method randomly initialize the grid with live bombs.
        /// The method should utilize the Difficulty property to determine 
        /// what percentage of the cells in gid will be set to "live" status
        /// 
        /// </summary>
        public void setupLiveNeighbors(int difficulty)
        {
            board.Difficulty = difficulty;
            // Create a random tool
            Random rnd = new Random();

            for (int x = 0; x < board.Difficulty; x++)
            {
                Boolean isSuccess = false;
                while (isSuccess == false)
                {
                    int newRow = rnd.Next(0, board.Size);
                    int newCol = rnd.Next(0, board.Size);
                    if (board.TheGrid[newCol, newRow].IsLive == false)
                    {
                        board.TheGrid[newCol, newRow].IsLive = true;
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
        }

        /// <summary>
        /// A method to calculate the live neighbors for every cell on the grid
        /// A cell should have between 0 and 8 live neighbors.
        /// If a cell itself is "live" then you can set the neighbor count to 9
        /// </summary>
        /// <returns></returns>
        public void calculateLiveNeighbors()
        {
            // we check for each individual cell
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    CellModel currentCell = board.TheGrid[i, j];

                    // if the current cell is a bomb. set its value to 9
                    if (currentCell.IsLive)
                    {
                        currentCell.NumNeighbors = 9;
                    }

                    // if not, for any neighbor near by is a bomb, increment the numNeighbors of current cell by 1
                    else
                    {
                        for (int r = -1; r < 2; r++)
                        {
                            for (int c = -1; c < 2; c++)
                            {
                                if (isSquareOnBoard(currentCell.ColNumber + r, currentCell.RowNumber + c) && board.TheGrid[currentCell.ColNumber + r, currentCell.RowNumber + c].IsLive)
                                {
                                    ++currentCell.NumNeighbors;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recursive private method that implements the flood fill algorithm.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="targetColor"></param>
        /// <param name="replacementColor"></param>
        public void FloodFill(int col, int row)
        {
            //Console.WriteLine("{0}, {1}", col, row);

            // Check boundary conditions and whether the cell is a wall.
            if (!isSquareOnBoard(col, row))
            {
                return;
            }


            // If the cell's num of neighbor is not 0 or the cell is visited, return immediately.
            if (board.TheGrid[col, row].NumNeighbors != 0 || board.TheGrid[col, row].IsVisited == true)
            {
                return;
            }


            // Change the color of the current cell to the replacement color.
            board.TheGrid[col, row].IsVisited = true;

            // Recursively flood fill the neighboring cells in all four directions.
            FloodFill(col, row - 1); // North          
            FloodFill(col + 1, row); // East            
            FloodFill(col, row + 1); // South
            FloodFill(col - 1, row); // West 
        }
    }
}
