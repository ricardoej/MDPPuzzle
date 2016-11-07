using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDPPuzzle
{
    public class Cell
    {
        public Cell(CellType type, double reward, double value)
        {
            this.Type = type;
            this.Reward = reward;
            this.Value = value;
        }

        public CellType Type { get; set; }

        public double Reward { get; set; }

        public double Value { get; set; }
    }
}
