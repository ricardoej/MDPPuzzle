using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDPPuzzle
{
    /// <summary>
    /// Interaction logic for Terrain.xaml
    /// </summary>
    public partial class Terrain : UserControl
    {
        private Dictionary<int, TerrainCell> terrainCells = new Dictionary<int, TerrainCell>();
        private const int ANIMATION_TIME_IN_SEC = 1;
        private bool runningPath = false;
        private List<int> path = new List<int>();
        private int currentPathIndex = -1;

        public TerrainDefinition CurrentTerrain { get; set; }

        public Terrain()
        {
            InitializeComponent();
        }

        public void Draw(TerrainDefinition terrainDefinition)
        {
            Clear();

            CurrentTerrain = terrainDefinition;

            for (int i = 0; i < terrainDefinition.Columns; i++)
            {
                this.terrainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.subtitleX.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < terrainDefinition.Rows; i++)
            {
                this.terrainGrid.RowDefinitions.Add(new RowDefinition());
                this.subtitleY.RowDefinitions.Add(new RowDefinition());
            }

            for (int col = 0; col < terrainDefinition.Columns; col++)
            {
                var labelX = new Label();
                labelX.Content = col;
                labelX.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                labelX.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetColumn(labelX, col);
                Grid.SetRow(labelX, 0);
                this.subtitleX.Children.Add(labelX);
            }

            for (int row = 0; row < terrainDefinition.Rows; row++)
            {
                var labelY = new Label();
                labelY.Content = row;
                labelY.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                labelY.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetColumn(labelY, 0);
                Grid.SetRow(labelY, row);
                this.subtitleY.Children.Add(labelY);
            }

            for (int col = 0; col < terrainDefinition.Columns; col++)
            {
                for (int row = 0; row < terrainDefinition.Rows; row++)
                {
                    var cell = new TerrainCell(terrainDefinition.GetCellType(col, row), col, row, terrainDefinition.GetCellReward(col, row), terrainDefinition.GetCellValue(col, row));
                    terrainCells.Add(terrainDefinition.ConvertCoordinateToPosition(col, row), cell);
                    cell.MouseDoubleClick += delegate
                    {
                        MoveRobotTo(cell.Column, cell.Row);
                    };

                    Grid.SetColumn(cell, col);
                    Grid.SetRow(cell, row);

                    this.terrainGrid.Children.Add(cell);
                }
            }

            TranslateTransform trans = new TranslateTransform();
            robot.RenderTransform = trans;
            this.robot.Visibility = System.Windows.Visibility.Visible;

            MoveRobotTo(0, 0);
        }

        private void Clear()
        {
            this.terrainGrid.ColumnDefinitions.Clear();
            this.terrainGrid.RowDefinitions.Clear();
            this.terrainGrid.Children.Clear();

            this.subtitleX.ColumnDefinitions.Clear();
            this.subtitleX.RowDefinitions.Clear();
            this.subtitleX.Children.Clear();

            this.subtitleY.ColumnDefinitions.Clear();
            this.subtitleY.RowDefinitions.Clear();
            this.subtitleY.Children.Clear();

            terrainCells.Clear();
        }

        public void MoveRobotTo(int col, int row, EventHandler completedEvent = null)
        {
            if (CurrentTerrain.GetCellType(col, row) != CellType.MOUNTAIN)
            {
                double cellCenterX = (col * (terrainGrid.ActualWidth / CurrentTerrain.Columns) + (terrainCells[CurrentTerrain.ConvertCoordinateToPosition(col, row)].ActualWidth / 2) - (robot.ActualWidth / 2));
                double cellCenterY = (row * (terrainGrid.ActualHeight / CurrentTerrain.Rows) + (terrainCells[CurrentTerrain.ConvertCoordinateToPosition(col, row)].ActualHeight / 2) - (robot.ActualHeight / 2));

                var transform = (robot.RenderTransform as TranslateTransform);

                double currentX = transform.X;
                double currentY = transform.Y;

                DoubleAnimation anim1 = new DoubleAnimation(currentX, cellCenterX, TimeSpan.FromSeconds(ANIMATION_TIME_IN_SEC));
                DoubleAnimation anim2 = new DoubleAnimation(currentY, cellCenterY, TimeSpan.FromSeconds(ANIMATION_TIME_IN_SEC));

                if (completedEvent != null)
                    anim1.Completed += completedEvent;

                transform.BeginAnimation(TranslateTransform.XProperty, anim1);
                transform.BeginAnimation(TranslateTransform.YProperty, anim2);

                CurrentRobotPosition = new Point(col, row);
            }
        }

        public void ShowPolicy(int[][] policy, double[] values)
        {
            for (int i = 0; i < policy.Length; i++)
			{
                TerrainCell cell = terrainCells[i];
                for (int j = 0; j < policy[i].Length; j++)
                {
                    if (policy[i][j] > 0)
                    {
                        cell.ShowAction((Actions)j);
                        cell.Value = values[i];
                    }
                }
			}
        }

        public void HidePolicy()
        {
            foreach (var item in terrainGrid.Children)
            {
                TerrainCell cell = item as TerrainCell;
                cell.HideAction();
            }
        }

        public void ExecutePolicy(int[][] CurrentPolicy)
        {
            path = GetPolicyPath(CurrentPolicy);

            if (path.Count > 0)
            {
                runningPath = true;
                ExecuteNextStateFromPath();
            }
        }

        private void ExecuteNextStateFromPath()
        {
            var cell = CurrentTerrain.ConvertPositionToCoordinate(path[currentPathIndex + 1]);
            MoveRobotTo((int)cell.X, (int)cell.Y,
                delegate
                {
                    currentPathIndex++;
                    if (path.Count > currentPathIndex + 1)
                    {
                        ExecuteNextStateFromPath();
                    }
                    else
                    {
                        path.Clear();
                        currentPathIndex = -1;
                        runningPath = false;

                        if (CurrentTerrain.IsGoal((int)CurrentRobotPosition.X, (int)CurrentRobotPosition.Y))
                        {
                            MessageBox.Show("O robô atingiu o objetivo!");
                        }
                        else
                        {
                            MessageBox.Show("Que pena! Não conseguimos atingir o objetivo!");
                        }
                    }
                }
            );
        }

        private List<int> GetPolicyPath(int[][] CurrentPolicy)
        {
            List<int> path = new List<int>();
            int lastPosition = CurrentTerrain.ConvertCoordinateToPosition((int)CurrentRobotPosition.X, (int)CurrentRobotPosition.Y);
            Random rand = new Random();
            bool end = false;

            while (!end)
            {
                if (CurrentPolicy[lastPosition].Contains(1))
                {
                    var action = 0;

                    for (int a = 0; a < CurrentPolicy[lastPosition].Length; a++)
                    {
                        if (CurrentPolicy[lastPosition][a] == 1)
                        {
                            action = a;
                            break;
                        }
                    }

                    var nextStates = new List<int>();
                    var transitions = CurrentTerrain.GetTransitions();
                    for (int i = 0; i < transitions.GetLength(0); i++)
                    {
                        if (transitions[lastPosition, action, i] > 0)
                        {
                            nextStates.Add(i);
                        }
                    }

                    if (nextStates.Count > 0)
                    {
                        double randValue = rand.NextDouble();
                        double probSum = 0;
                        foreach (var state in nextStates)
                        {
                            probSum += transitions[lastPosition, action, state];
                            if (randValue <= probSum)
                            {
                                path.Add(state);
                                lastPosition = state;
                                break;
                            }
                        }
                    }
                    else
                    {
                        end = true;
                    }
                }
                else
                {
                    end = true;
                }
            }

            return path;
        }

        public Point CurrentRobotPosition { get; set; }
    }
}