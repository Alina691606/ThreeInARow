using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ThreeInARow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int pointsCount = 0;
        DispatcherTimer timer;
        double timeLeft;
        double timeMultiplier;
        GameGrid gameGrid;
        public MainWindow()
        {
            InitializeComponent();
         
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            gameGrid = new GameGrid();
            RootGrid.Children.Add( gameGrid );
            gameGrid.AddPoints += ChangePointsLabel;
            buttonStart.Visibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timeLeft = 60;
            timeMultiplier = 1;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if(timeLeft > 0)
            {
                timeMultiplier *= 1.05;
                timeLeft -= 1 * timeMultiplier;
                labelTimer.Content = "Осталось " + (int)timeLeft + " сек";
            }
            else
            {
                timer.Stop();
                timeLeft = 60;
                gameGrid.AddPoints -= ChangePointsLabel;
                RootGrid.Children.Remove(gameGrid);
                buttonStart.Visibility = Visibility.Visible;
                labelTimer.Content = "Осталось 0 сек";
                
                MessageBox.Show("Вы набрали " + pointsCount + " очков!");

                pointsCount = 0;
            }
        }

        void ChangePointsLabel(int points)
        {
            pointsCount += points;
            labelPoints.Content = pointsCount;
            timeLeft += points;
            labelTimer.Content = "Осталось " + (int)timeLeft + " сек";
        }
    }
}
