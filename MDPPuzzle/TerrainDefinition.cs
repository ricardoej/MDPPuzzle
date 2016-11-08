using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MDPPuzzle
{
    public class TerrainDefinition
    {
        public Dictionary<int, Cell> Cells { get; set; }

        public int Columns { get; set; }

        public int Rows { get; set; }

        public double DefaultReward { get; set; }

        public double GoingAheadProbability { get; set; }

        public double GoingLeftProbability { get; set; }

        public double GoingBackProbability { get; set; }

        public double GoingRightProbability { get; set; }

        public double IsAtGoalProbability { get; set; }

        public double IsAtCampProbability { get; set; }

        public double IsInForestProbability { get; set; }

        public double IsInPathProbability { get; set; }

        public double Gama { get; set; }

        public int ActionsNumber
        {
            get
            {
                return Enum.GetNames(typeof(Actions)).Length;
            }
        }

        public int ObservationsNumber
        {
            get
            {
                return Enum.GetNames(typeof(Observations)).Length;
            }
        }

        public TerrainDefinition()
        {
            this.Columns = 5;
            this.Rows = 5;
            this.Cells = new Dictionary<int, Cell>();
        }

        public void SetCellAttributes(int x, int y, double reward = 0, double value = 0, CellType type = CellType.PATH)
        {
            if (x < this.Columns && y < this.Rows)
            {
                var position = ConvertCoordinateToPosition(x, y);
                if (!this.Cells.ContainsKey(position))
                {
                    this.Cells.Add(position, new Cell(type, reward, value));
                }
            }
            else
            {
                throw new Exception(string.Format(
                    "Não é possível adicionar uma célula de tipo {0} em ({1}, {2}) porque essa coordenada não existe no mapa (Lembre-se: o mapa possui {3} colunas e {4} linhas)",
                    type, x, y, Columns, Rows));
            }
        }

        public int ConvertCoordinateToPosition(int x, int y)
        {
            return x + (y * this.Columns);
        }

        public Point ConvertPositionToCoordinate(int pos)
        {
            return new Point(pos % this.Columns, (int)(pos/this.Columns));
        }

        public CellType GetCellType(int col, int row)
        {
            var index = ConvertCoordinateToPosition(col, row);
            if (Cells.ContainsKey(index))
                return Cells[index].Type;

            return CellType.PATH;
        }

        public double GetCellReward(int col, int row)
        {
            var index = ConvertCoordinateToPosition(col, row);
            if (Cells.ContainsKey(index))
                return Cells[index].Reward;

            return DefaultReward;
        }

        public double GetCellValue(int col, int row)
        {
            var index = ConvertCoordinateToPosition(col, row);
            if (Cells.ContainsKey(index))
                return Cells[index].Value;

            return 0;
        }

        public bool IsObstacle(int col, int row)
        {
            return GetCellType(col, row) == CellType.MOUNTAIN;
        }

        public bool IsObstacle(int pos)
        {
            var coord = ConvertPositionToCoordinate(pos);
            return GetCellType((int)coord.X, (int)coord.Y) == CellType.MOUNTAIN;
        }

        public bool IsGoal(int col, int row)
        {
            return GetCellType(col, row) == CellType.GOAL;
        }

        public bool IsGoal(int pos)
        {
            var coord = ConvertPositionToCoordinate(pos);
            return GetCellType((int)coord.X, (int)coord.Y) == CellType.GOAL;
        }

        public bool IsCamp(int col, int row)
        {
            return GetCellType(col, row) == CellType.CAMP;
        }

        public bool IsCamp(int pos)
        {
            var coord = ConvertPositionToCoordinate(pos);
            return GetCellType((int)coord.X, (int)coord.Y) == CellType.CAMP;
        }

        public bool IsForest(int col, int row)
        {
            return GetCellType(col, row) == CellType.FOREST;
        }

        public bool IsForest(int pos)
        {
            var coord = ConvertPositionToCoordinate(pos);
            return GetCellType((int)coord.X, (int)coord.Y) == CellType.FOREST;
        }

        public bool IsPath(int col, int row)
        {
            return GetCellType(col, row) == CellType.PATH;
        }

        public bool IsPath(int pos)
        {
            var coord = ConvertPositionToCoordinate(pos);
            return GetCellType((int)coord.X, (int)coord.Y) == CellType.PATH;
        }

        public double[, ,] GetTransitions()
        {
            var statesNumber = this.Columns * this.Rows;
            var actionsNumber = this.ActionsNumber;
            var transitions = new double[statesNumber, actionsNumber, statesNumber];

            // Calcula as probabilidades de transição
            for (int s0 = 0; s0 < statesNumber; s0++)
            {
                if (!this.IsObstacle(s0) && !this.IsGoal(s0) && !this.IsCamp(s0))
                {
                    for (int a = 0; a < actionsNumber; a++)
                    {
                        switch (a)
                        {
                            case (int)Actions.EAST:
                                // Go ahead
                                SetEastProb(transitions, s0, a, this.GoingAheadProbability);

                                // Go left
                                SetNorthProb(transitions, s0, a, this.GoingLeftProbability);

                                // Go right
                                SetSouthProb(transitions, s0, a, this.GoingRightProbability);

                                // Go back
                                SetWestProb(transitions, s0, a, this.GoingBackProbability);
                                break;
                            case (int)Actions.WEST:
                                // Go back
                                SetEastProb(transitions, s0, a, this.GoingBackProbability);

                                // Go right
                                SetNorthProb(transitions, s0, a, this.GoingRightProbability);

                                // Go left
                                SetSouthProb(transitions, s0, a, this.GoingLeftProbability);

                                // Go ahead
                                SetWestProb(transitions, s0, a, this.GoingAheadProbability);
                                break;
                            case (int)Actions.NORTH:
                                // Go right
                                SetEastProb(transitions, s0, a, this.GoingRightProbability);

                                // Go ahead
                                SetNorthProb(transitions, s0, a, this.GoingAheadProbability);

                                // Go back
                                SetSouthProb(transitions, s0, a, this.GoingBackProbability);

                                // Go left
                                SetWestProb(transitions, s0, a, this.GoingLeftProbability);
                                break;
                            case (int)Actions.SOUTH:
                                // Go left
                                SetEastProb(transitions, s0, a, this.GoingLeftProbability);

                                // Go back
                                SetNorthProb(transitions, s0, a, this.GoingBackProbability);

                                // Go ahead
                                SetSouthProb(transitions, s0, a, this.GoingAheadProbability);

                                // Go right
                                SetWestProb(transitions, s0, a, this.GoingRightProbability);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (this.IsGoal(s0) || this.IsCamp(s0))
                {
                    transitions[s0, (int)Actions.EAST, s0] = 1;
                    transitions[s0, (int)Actions.WEST, s0] = 1;
                    transitions[s0, (int)Actions.NORTH, s0] = 1;
                    transitions[s0, (int)Actions.SOUTH, s0] = 1;
                }
            }

            return transitions;
        }

        private void SetEastProb(double[, ,] transitions, int s0, int a, double prob)
        {
            var sCoordinate = this.ConvertPositionToCoordinate(s0);
            var candidateS1 = this.ConvertCoordinateToPosition((int)sCoordinate.X + 1, (int)sCoordinate.Y);
            var s1 = sCoordinate.X < this.Columns - 1 && !this.IsObstacle(candidateS1) ? candidateS1 : s0;
            transitions[s0, a, s1] += prob;
        }

        private void SetSouthProb(double[, ,] transitions, int s0, int a, double prob)
        {
            var sCoordinate = this.ConvertPositionToCoordinate(s0);
            var candidateS1 = this.ConvertCoordinateToPosition((int)sCoordinate.X, (int)sCoordinate.Y + 1);
            var s1 = sCoordinate.Y < this.Rows - 1 && !this.IsObstacle(candidateS1) ? candidateS1 : s0;
            transitions[s0, a, s1] += prob;
        }

        private void SetWestProb(double[, ,] transitions, int s0, int a, double prob)
        {
            var sCoordinate = this.ConvertPositionToCoordinate(s0);
            var candidateS1 = this.ConvertCoordinateToPosition((int)sCoordinate.X - 1, (int)sCoordinate.Y);
            var s1 = sCoordinate.X > 0 && !this.IsObstacle(candidateS1) ? candidateS1 : s0;
            transitions[s0, a, s1] += prob;
        }

        private void SetNorthProb(double[, ,] transitions, int s0, int a, double prob)
        {
            var sCoordinate = this.ConvertPositionToCoordinate(s0);
            var candidateS1 = this.ConvertCoordinateToPosition((int)sCoordinate.X, (int)sCoordinate.Y - 1);
            var s1 = sCoordinate.Y > 0 && !this.IsObstacle(candidateS1) ? candidateS1 : s0;
            transitions[s0, a, s1] += prob;
        }

        public double[,] GetRewards()
        {
            var rewards = new double[this.Columns * this.Rows, this.ActionsNumber];

            for (int i = 0; i < rewards.GetLength(0); i++)
            {
                for (int j = 0; j < this.ActionsNumber; j++)
                {
                    var coord = ConvertPositionToCoordinate(i);
                    rewards[i, j] = GetCellReward((int)coord.X, (int)coord.Y);
                }
            }

            return rewards;
        }

        public double[, ,] GetObservations()
        {
            var observationsNumber = this.ObservationsNumber;
            var actionsNumber = this.ActionsNumber;
            var statesNumber = this.Columns * this.Rows;
            var observations = new double[observationsNumber, actionsNumber, statesNumber];

            for (int a = 0; a < this.ActionsNumber; a++)
            {
                for (int s = 0; s < this.ActionsNumber; s++)
                {
                    if (this.IsObstacle(s))
                    {
                        continue;
                    }
                    if (this.IsGoal(s))
                    {
                        this.SetObservationProbability(observations, (int) Observations.GOAL, a, s, this.IsAtGoalProbability);
                    }
                    else if (this.IsCamp(s))
                    {
                        this.SetObservationProbability(observations, (int) Observations.CAMP, a, s, this.IsAtCampProbability);
                    }
                    else if (this.IsForest(s))
                    {
                        this.SetObservationProbability(observations, (int) Observations.FOREST, a, s, this.IsInForestProbability);
                    }
                    else if (this.IsPath(s))
                    {
                        this.SetObservationProbability(observations, (int) Observations.PATH, a, s, this.IsInPathProbability);
                    }
                }
            }

            return observations;
        }

        private void SetObservationProbability(double[, ,] observations, int a, int o, int s, double prob)
        {
            //var coord = ConvertPositionToCoordinate(s);
        }

        public double[] GetValues()
        {
            var values = new double[this.Columns * this.Rows];

            for (int i = 0; i < values.Length; i++)
            {
                var coord = ConvertPositionToCoordinate(i);
                values[i] = GetCellValue((int)coord.X, (int)coord.Y);
            }

            return values;
        }
    }
}
