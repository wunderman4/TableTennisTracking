using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TableTennisTracker.Models;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for PlotHitLocations.xaml
    /// </summary>
    public partial class PlotHitLocations : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string CallPage;
        public Game Game;
        public List<HitLocation> Bounces;

        public PlotHitLocations(List<HitLocation> _bounces, Game _game, string _callPage)
        {
            CallPage = _callPage;
            Game = _game;
            Bounces = _bounces;

            InitializeComponent();

            PlotXYData();
        }

        // Create xyData list from Bounces, send to XAML plot
        public void PlotXYData()
        {
            List<KeyValuePair<float, float>> xyData = new List<KeyValuePair<float, float>>();

            for (int i = 0; i < Bounces.Count; i++)
            {
                xyData.Add(new KeyValuePair<float, float>(Bounces[i].X, Bounces[i].Y));
            }

            chart0.DataContext = xyData;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            if (CallPage == "GameSummary")
            {
                NavigationService.Navigate(new GameSummary(Game, Bounces));
            }
            else if (CallPage == "Leaderboard")
            {
                NavigationService.Navigate(new Leaderboard());
            }
        }
    }
}
