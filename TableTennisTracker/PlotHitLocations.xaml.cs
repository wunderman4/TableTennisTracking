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
using TableTennisTracker.ModelViews;
using TableTennisTracker.Services;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for PlotHitLocations.xaml
    /// </summary>
    public partial class PlotHitLocations : Page
    {
        public string CallPage;
        public GamesView Game;
        public Game InGame;
        public List<GamesView> Games;
        GameService gs = new GameService();

        public PlotHitLocations(Game _game, string _callPage)
        {
            CallPage = _callPage;
            InGame = _game;

            InitializeComponent();

            if (InGame == null)
            {
                GetAllGames();
            }
            else
            {
                HitLocationButton.Visibility = Visibility.Collapsed;
                PickGame.Visibility = Visibility.Collapsed;
                Game = gs.GetGame(InGame.Id);
                chart0.Visibility = Visibility.Visible;
                PlotXYData();
            }
        }

        private async void GetAllGames()
        {

            Task<List<GamesView>> AllGames = Task.Factory.StartNew(() => GetAllGamesWorker());
            Games = await AllGames;
            PickGameList.ItemsSource = Games;
            PickGameList.Visibility = Visibility.Visible;
            PickGame.Visibility = Visibility.Visible;
            PBar.Visibility = Visibility.Collapsed;
            
        }

        private List<GamesView> GetAllGamesWorker()
        {
            return gs.GetGames();
        }


        // Create xyData list from Bounces, send to XAML plot
        public void PlotXYData()
        {
            List<KeyValuePair<float, float>> xyData = new List<KeyValuePair<float, float>>();

            for (int i = 0; i < Game.GameHitLocations.Count; i++)
            {
                xyData.Add(new KeyValuePair<float, float>(Game.GameHitLocations[i].X, Game.GameHitLocations[i].Z));
            }

            chart0.DataContext = xyData;
        }

        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {
            GamesView ChosenGame = (GamesView)PickGameList.SelectedItem;
            PickGame.Visibility = Visibility.Collapsed;
            chart0.Visibility = Visibility.Visible;
            HitLocationButton.Visibility = Visibility.Collapsed;
            PickNewGameButton.Visibility = Visibility.Visible;
            Game = gs.GetGame(ChosenGame.Id);
            PlotXYData();
        }

        private void PickNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            chart0.Visibility = Visibility.Collapsed;
            PickNewGameButton.Visibility = Visibility.Collapsed;
            PickGame.Visibility = Visibility.Visible;
            HitLocationButton.Visibility = Visibility.Visible;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            if (CallPage == "GameSummary")
            {
                NavigationService.Navigate(new GameSummary(InGame));
            }
            else if (CallPage == "Leaderboard")
            {
                NavigationService.Navigate(new Leaderboard());
            }
        }
    }
}
