using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP
{
    public abstract class MDPSolver
    {
        public int StatesNumber { get; set; }

        public int ActionsNumber { get; set; }

        public double[,,] Transitions { get; set; }

        public double[,] Rewards { get; set; }

        public double Gama { get; set; }

        public MDPSolver(int statesNumber, int actionsNumber, double[,,] transitions, double[,] rewards, double gama)
        {
            this.StatesNumber = statesNumber;
            this.ActionsNumber = actionsNumber;
            this.Transitions = transitions;
            this.Rewards = rewards;
            this.Gama = gama;
        }

        public abstract int[][] Solve(ref double[] values);
    }
}
