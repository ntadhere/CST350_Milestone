// Truong Anh Dao Nguyen
// CST-250
// 02/09/2024
// Milestone 2
// This is my own word

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CST350_Milestone.Models
{ 
    /// <summary>
    /// Class that represents an individual cell on the board
    /// </summary>
    public class CellModel
    {
        // row and col are the cell's location on the grid
        public int ColNumber { get; set; }
        public int RowNumber { get; set; }

        // number of neighbors that are live
        public int NumNeighbors { get; set; }
        // T/F is the user visited this location
        public bool IsVisited { get; set; }
        // T/F is this location have bome
        public bool IsLive { get; set; }
        // constructor
        public CellModel(int c, int r)
        {
            RowNumber = r;
            ColNumber = c;
        }
    }
}
