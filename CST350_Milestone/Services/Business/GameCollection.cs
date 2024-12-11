using CST350_Milestone.Models;
using CST350_Milestone.Services.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace CST350_Milestone.Services.Business
{
    public class GameCollection
    {
        private readonly GameDAO _gameDAO;

        // This is an in-memory list of users. Later this will be a db connection.
        private BoardModel board = new BoardModel();
        //private List<CellModel> cells = new List<CellModel>();

        // This line allows other parts of your program to access the board instance
        // without allowing them to modify it directly (since there is no set accessor).
        // It's a way to expose board for read-only access.
        public BoardModel Board => board; //Public getter to expose BoardModel


        // Constructor to inject the DAO dependency
        public GameCollection()
        {
            _gameDAO = new GameDAO();
        }

        /// <summary>
        /// This method help to generate a new board for game then return the model
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public BoardModel GenerateBoard (int size)
        {
            int id = 0;
            board.Size = size;
            // we must initialize the array to avoid Null Exception errors
            board.TheGrid = new CellModel[size, size];

            // Initialize CellListData before adding items to avoid null reference
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
        /// check if the square on the board or not
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsSquareOnBoard(int col, int row)
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

        /// <summary>
        /// this method helps to check the win condition for the game.
        /// </summary>
        /// <returns></returns>
        public Boolean IsWin()
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
        public void SetupLiveNeighbors(int difficulty)
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
        public void CalculateLiveNeighbors()
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
                                if (IsSquareOnBoard(currentCell.ColNumber + r, currentCell.RowNumber + c) && board.TheGrid[currentCell.ColNumber + r, currentCell.RowNumber + c].IsLive)
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
            if (!IsSquareOnBoard(col, row))
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

        /// <summary>
        /// Method to calculate the score
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <param name="boardSize"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public int gameCalculation(int elapsedTime, int boardSize, int difficulty)
        {
            int score = (boardSize * difficulty * 100) + elapsedTime;
            return Math.Max(score, 0);
        }

        /// <summary>
        /// Save the game state to the database
        /// </summary>
        /// <param name="userId">The ID of the user saving the game</param>
        /// <param name="gameDataJson">The serialized game state</param>
        /// <returns></returns>
        public bool SaveGame(int userId, string gameDataJson)
        {
            try
            {
                // Call the DAO to save the game
                return _gameDAO.SaveGame(userId, gameDataJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }
    }
}
