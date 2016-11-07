using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP
{
    public class COMDPSolver: MDPSolver
    {
        public COMDPSolver(int statesNumber, int actionsNumber, double[,,] transitions, double[,] rewards, double gama)
            :base(statesNumber, actionsNumber, transitions, rewards, gama)
        { }

        public override int[][] Solve(ref double[] values)
        {
            var V = values;
            var VLinha = new double[StatesNumber];
            var policy = new int[StatesNumber][];

            do
            {
                Array.Copy(V, VLinha, StatesNumber);

                var Q = new double[StatesNumber][];

                for (int s = 0; s < StatesNumber; s++)
                {
                    Q[s] = new double[ActionsNumber];
                    policy[s] = new int[ActionsNumber];
                    
                    for (int a = 0; a < ActionsNumber; a++)
                    {
                        double sum = 0;
                        bool hasTransitions = false;
                        for (int sLinha = 0; sLinha < StatesNumber; sLinha++)
                        {
                            sum += Transitions[s, a, sLinha] * V[sLinha];
                            if (Transitions[s, a, sLinha] != 0)
                                hasTransitions = true;
                        }

                        if (hasTransitions)
                        {
                            Q[s][a] = Rewards[s, a] + (Gama * sum);

                            if (Q[s][a] >= V[s])
                            {
                                V[s] = Q[s][a];
                                policy[s][a] = 1;
                            } 
                        }
                    }
                }

            } while (!Converged(V, VLinha));

            return policy;
        }

        private bool Converged(double[] v0, double[] v1)
        {
            for (int i = 0; i < v0.Length; i++)
            {
                if (Math.Abs(v0[i] - v1[i]) > 0.001)
                    return false;
            }

            return true;
        }
    }
}
