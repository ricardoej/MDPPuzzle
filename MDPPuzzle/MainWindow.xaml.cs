using MDP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDPPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentMapPath { get; set; }

        public int[][] CurrentPolicy { get; set; }

        public double[] CurrentValues { get; set; }

        private TerrainDefinition Cfg
        {
            get { return this.terrain.CurrentTerrain; }
        }

        public MainWindow()
        {
            InitializeComponent();
            CurrentMapPath = "presentation.terrain";

            LoadMap(CurrentMapPath);
        }

        private void LoadMap(string mapPath)
        {
            try
            {
                CurrentPolicy = null;
                showPolicyMenuItem.IsChecked = false;
                this.terrain.Draw(LoadTerrainDefinition(mapPath));
            }
            catch (Exception e)
            {
                MessageBox.Show("O mapa não pode ser carregado \n\n" + e.Message);
            }
        }

        private TerrainDefinition LoadTerrainDefinition(string fileName)
        {
            var terrainDefinition = new TerrainDefinition();

            using (StreamReader sr = new StreamReader(fileName))
            {
                int validLine = 0;
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                    {
                        validLine += 1;
                        if (validLine == 1)
                        {
                            string[] lineSplitted = line.Split(' ');
                            terrainDefinition.Columns = int.Parse(lineSplitted[0]);
                            terrainDefinition.Rows = int.Parse(lineSplitted[1]);
                        }
                        else if (validLine == 2)
                        {
                            terrainDefinition.DefaultReward = double.Parse(line);
                        }
                        else if (validLine == 3)
                        {
                            string[] lineSplitted = line.Split(' ');
                            terrainDefinition.GoingAheadProbability = double.Parse(lineSplitted[0]);
                            terrainDefinition.GoingRightProbability = double.Parse(lineSplitted[1]);
                            terrainDefinition.GoingBackProbability = double.Parse(lineSplitted[2]);
                            terrainDefinition.GoingLeftProbability = double.Parse(lineSplitted[3]);
                        }
                        else if (validLine == 4)
                        {
                            terrainDefinition.Gama = double.Parse(line);
                        }
                        else
                        {
                            string[] lineSplitted = line.Split(' ');
                            terrainDefinition.SetCellAttributes(int.Parse(lineSplitted[0]), 
                                int.Parse(lineSplitted[1]), 
                                double.Parse(lineSplitted[2]),
                                double.Parse(lineSplitted[3]),
                                (CellType)Enum.Parse(typeof(CellType), lineSplitted[4]));
                        }
                    }
                }
            }

            return terrainDefinition;
        }

        private void OpenMapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentMapPath = openFileDialog.FileName;
                LoadMap(CurrentMapPath);
            }
        }

        private void ReloadMapMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurrentMapPath))
            {
                LoadMap(CurrentMapPath);
            }
        }

        private void SolvePolicyMenuItemClick(object sender, RoutedEventArgs e)
        {
            var transitions = Cfg.GetTransitions();
            double[,] rewards = Cfg.GetRewards();
            double[] values = new double[Cfg.Columns * Cfg.Rows];
            Array.Copy(Cfg.GetValues(), values, Cfg.GetValues().Length);
            MDPSolver solver = new COMDPSolver(Cfg.Columns * Cfg.Rows, 4, transitions, rewards, Cfg.Gama);
            CurrentPolicy = solver.Solve(ref values);
            CurrentValues = values;

            ShowPolicy();
        }

        private void ShowPolicy()
        {
            showPolicyMenuItem.IsChecked = false;
            showPolicyMenuItem.IsChecked = true;
        }

        private void ShowPolicyMenuItemChecked(object sender, RoutedEventArgs e)
        {
            if (CurrentPolicy != null)
            {
                terrain.ShowPolicy(CurrentPolicy, CurrentValues); 
            }
        }

        private void HidePolicyMenuItemUnchecked(object sender, RoutedEventArgs e)
        {
            terrain.HidePolicy();
        }

        private void RunMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (CurrentPolicy != null)
                terrain.ExecutePolicy(CurrentPolicy);
            else
                MessageBox.Show("Não existe nenhuma política calculada");
        }
    }
}
