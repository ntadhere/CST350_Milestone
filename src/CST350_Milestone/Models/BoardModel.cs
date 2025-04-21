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
    }
}
