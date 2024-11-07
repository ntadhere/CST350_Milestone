// Truong Anh Dao Nguyen
// CST-250
// 02/09/2024
// Milestone 2
// This is my own word

using CST350_Milestone.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CST350_Milestone.Models
{
    public class BoardModel
    {
        [Required]
        [Display(Name = "Board game size")]
        public int Size { get; set; }               // the board is always square.

        [Required]
        [Display(Name = "Number of bombs")]
        public int Difficulty { get; set; }      // A percentage of cells that will be set to "live" status

        // 2d array of Cell objects
        public CellModel[,] TheGrid { get; set; }
        public List<CellModel> CellListData { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BoardModel()
        {
        }


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

        //// constructor
        //public BoardModel(int size)
        //{
        //    Size = size;
        //    // we must initialize the array to avoid Null Exception errors
        //    TheGrid = new CellModel[Size, Size];
        //    for (int i = 0; i < Size; i++)
        //    {
        //        for (int j = 0; j < Size; j++)
        //        {
        //            TheGrid[i, j] = new CellModel(i, j);
        //        }
        //    }
        //}

        ///// <summary>
        ///// check if the square on the board or not
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="col"></param>
        ///// <returns></returns>
        //public bool isSquareOnBoard(int col, int row)
        //{
        //    // -9, 5 is out of bound
        //    if (row < 0 || row > Size - 1 || col < 0 || col > Size - 1)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public int getSquareOnBoard(int order)
        //{
        //    int temp = 0;
        //    for (int i = 0; i < Size; i++)
        //    {
        //        for (int j = 0; j < Size; j++)
        //        {
        //            if (temp == order)
        //            {
        //                return TheGrid[i, j].NumNeighbors;
        //            }
        //            temp++;
        //        }
        //    }
        //    return -1;
        //}

        ///// <summary>
        ///// this method helps to check the win condition for the game.
        ///// </summary>
        ///// <returns></returns>
        //public Boolean WinCondition()
        //{
        //    int nonRevealCell = 0;
        //    Boolean isWin = false;

        //    // find the number of non-revealed cell
        //    for (int i = 0; i < Size; i++)
        //    {
        //        for (int j = 0; j < Size; j++)
        //        {
        //            if (TheGrid[i, j].IsVisited == false)
        //            {
        //                nonRevealCell++;
        //            }
        //        }
        //    }
        //    // if the number of bomb placed equal to the number of non-reveal cell
        //    if (Difficulty == nonRevealCell)
        //    {
        //        isWin = true;
        //    }
        //    return isWin;
        //}

        ///// <summary>
        ///// method randomly initialize the grid with live bombs.
        ///// The method should utilize the Difficulty property to determine 
        ///// what percentage of the cells in gid will be set to "live" status
        ///// 
        ///// </summary>
        //public void setupLiveNeighbors(int difficulty)
        //{
        //    Difficulty = difficulty;
        //    // Create a random tool
        //    Random rnd = new Random();

        //    for (int x = 0; x < Difficulty; x++)
        //    {
        //        Boolean isSuccess = false;
        //        while (isSuccess == false)
        //        {
        //            int newRow = rnd.Next(0, Size);
        //            int newCol = rnd.Next(0, Size);
        //            if (TheGrid[newCol, newRow].IsLive == false)
        //            {
        //                TheGrid[newCol, newRow].IsLive = true;
        //                isSuccess = true;
        //            }
        //            else
        //            {
        //                isSuccess = false;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// A method to calculate the live neighbors for every cell on the grid
        ///// A cell should have between 0 and 8 live neighbors.
        ///// If a cell itself is "live" then you can set the neighbor count to 9
        ///// </summary>
        ///// <returns></returns>
        //public void calculateLiveNeighbors()
        //{
        //    // we check for each individual cell
        //    for (int i = 0; i < Size; i++)
        //    {
        //        for (int j = 0; j < Size; j++)
        //        {
        //            CellModel currentCell = TheGrid[i, j];

        //            // if the current cell is a bomb. set its value to 9
        //            if (currentCell.IsLive)
        //            {
        //                currentCell.NumNeighbors = 9;
        //            }

        //            // if not, for any neighbor near by is a bomb, increment the numNeighbors of current cell by 1
        //            else
        //            {
        //                for (int r = -1; r < 2; r++)
        //                {
        //                    for (int c = -1; c < 2; c++)
        //                    {
        //                        if (isSquareOnBoard(currentCell.ColNumber + r, currentCell.RowNumber + c) && TheGrid[currentCell.ColNumber + r, currentCell.RowNumber + c].IsLive)
        //                        {                                 
        //                                ++currentCell.NumNeighbors;                                   
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Recursive private method that implements the flood fill algorithm.
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="col"></param>
        ///// <param name="targetColor"></param>
        ///// <param name="replacementColor"></param>
        //public void FloodFill(int col, int row)
        //{
        //    //Console.WriteLine("{0}, {1}", col, row);

        //    // Check boundary conditions and whether the cell is a wall.
        //    if (!isSquareOnBoard(col,row))
        //    {
        //        return;
        //    }


        //    // If the cell's num of neighbor is not 0 or the cell is visited, return immediately.
        //    if (TheGrid[col, row].NumNeighbors != 0 || TheGrid[col, row].IsVisited == true)
        //    {
        //        return;
        //    }


        //    // Change the color of the current cell to the replacement color.
        //    TheGrid[col, row].IsVisited = true;

        //    // Recursively flood fill the neighboring cells in all four directions.
        //    FloodFill(col, row - 1); // North          
        //    FloodFill(col + 1, row); // East            
        //    FloodFill(col, row + 1); // South
        //    FloodFill(col - 1, row); // West 
        //}
    }
}
