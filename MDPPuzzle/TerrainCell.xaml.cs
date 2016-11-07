using System;
using System.Collections.Generic;
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
    /// Interaction logic for TerrainCell.xaml
    /// </summary>
    public partial class TerrainCell : UserControl
    {
        private CellType type;

        public int Column { get; set; }
        public int Row { get; set; }
        public Actions? CurrentAction { get; set; }

        public TerrainCell(CellType type, int col, int row, double reward, double value)
        {
            InitializeComponent();

            this.type = type;
            this.Column = col;
            this.Row = row;
            this.Reward = reward;
            this.Value = value;

            this.rewardLabel.Foreground = this.Reward >= 0 ? Brushes.Blue : Brushes.Red;
            if (type == CellType.CAMP || type == CellType.GOAL)
                info1.Visibility = System.Windows.Visibility.Collapsed;

            Draw();
        }

        private void Draw()
        {
            SetImageType();
        }

        private void SetImageType()
        {
            switch (type)
            {
                case CellType.PATH:
                    break;
                case CellType.GOAL:
                    this.typeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/MDPPuzzle;component/Images/base.png"));
                    break;
                case CellType.FOREST:
                    this.typeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/MDPPuzzle;component/Images/forest.png"));
                    break;
                case CellType.MOUNTAIN:
                    this.typeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/MDPPuzzle;component/Images/mountain.png"));
                    info1.Visibility = System.Windows.Visibility.Collapsed;
                    info2.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case CellType.CAMP:
                    this.typeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/MDPPuzzle;component/Images/camp.png"));
                    break;
                default:
                    break;
            }
        }

        public double Reward
        {
            get { return (double)GetValue(RewardProperty); }
            set { SetValue(RewardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Reward.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RewardProperty =
            DependencyProperty.Register("Reward", typeof(double), typeof(TerrainCell), new PropertyMetadata(0.0));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, Math.Round(value)); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(TerrainCell), new PropertyMetadata(0.0));

        public void ShowAction(Actions action)
        {
            if (this.type != CellType.GOAL && this.type != CellType.MOUNTAIN)
            {
                HideAction();
                SetActionVisibility(action, System.Windows.Visibility.Visible);
                CurrentAction = action; 
            }
        }

        public void HideAction()
        {
            if (CurrentAction.HasValue)
            {
                SetActionVisibility(CurrentAction.Value, System.Windows.Visibility.Collapsed);
                CurrentAction = null;
            }
        }

        private void SetActionVisibility(Actions action, Visibility visibility)
        {
            switch (action)
            {
                case Actions.NORTH:
                    arrowUp.Visibility = visibility;
                    break;
                case Actions.SOUTH:
                    arrowDown.Visibility = visibility;
                    break;
                case Actions.EAST:
                    arrowRight.Visibility = visibility;
                    break;
                case Actions.WEST:
                    arrowLeft.Visibility = visibility;
                    break;
                default:
                    break;
            }
        }
    }
}
