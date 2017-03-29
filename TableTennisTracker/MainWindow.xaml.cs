using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new Splash();

            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {

        }


        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DemoItemsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;

            this.kinectRegion.InputPointerManager.CompleteGestures();
        }

        private async void AddPlayer(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new NewPlayer();
        }

        private async void NewGame(object sender, RoutedEventArgs e)
        {
            await Task.Delay(50);
            Main.Content = new SelectPlayers();
        }

        private async void Leaderboard(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new Leaderboard();
        }

        private async void Splash(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new Splash();
        }

        private async void GameTest(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new GameTest();
        }

        private async void Calibration(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new Calibration();
        }


        private async void ManagePlayers(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            Main.Content = new ManagePlayers();
        }
    }
}
