using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPPuzzle
{
    public class POMDPPolicyItem
    {
        private Dictionary<Observations, int> observationsToNode;
        private Dictionary<int, double> stateValues;

        public Actions Action { get; set; }

        public POMDPPolicyItem(Actions action)
        {
            this.Action = action;
            this.observationsToNode = new Dictionary<Observations, int>();
            this.stateValues = new Dictionary<int, double>();
        }

        public void AddObservationToNode(Observations observation, int node)
        {
            observationsToNode.Add(observation, node);
        }

        public void AddStateValue(int state, double value)
        {
            stateValues.Add(state, value);
        }

        public int GetNode(Observations observation)
        {
            return observationsToNode[observation];
        }

        public double GetValue(int state)
        {
            return stateValues[state];
        }
    }
}
