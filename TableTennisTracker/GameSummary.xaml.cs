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
using TableTennisTracker.Models;
using TableTennisTracker.Services;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for GameSummary.xaml
    /// </summary>
    public partial class GameSummary : Page
    {
        Game PastGame = null;
        Player PlayerOne;
        Player PlayerTwo;
        public List<HitLocation> Bounces;

        public GameSummary(Game _game, List<HitLocation> _bounces)
        {
            PlayerService ps = new PlayerService();

            InitializeComponent();
            PastGame = _game;
            Bounces = _bounces;

            PlayerOne = ps.GetPlayer(PastGame.Player1.Id);
            PlayerTwo = ps.GetPlayer(PastGame.Player2.Id);

            //PlayerOne DataContext
            P1UserNameDisplay.DataContext = PlayerOne;
            P1Score.DataContext = PastGame;
            P1Wins.DataContext = PlayerOne;
            P1Losses.DataContext = PlayerOne;

            //PlayerTwo DataContext
            P2UserNameDisplay.DataContext = PlayerTwo;
            P2Score.DataContext = PastGame;
            P2Wins.DataContext = PlayerTwo;
            P2Losses.DataContext = PlayerTwo;

            VolleyHits.DataContext = PastGame;
            VolleyTime.DataContext = PastGame;
            FastestHit.DataContext = PastGame;

            // Determine winner
            if (PastGame.Player1Score > PastGame.Player2Score)
            {
                GameWinner.DataContext = PlayerOne;
            }
            else
            {
                GameWinner.DataContext = PlayerTwo;
            }
            ColorShift();
        }

        private async void ColorShift()
        {
            while(true)
            {
                GameWinner.Foreground = new SolidColorBrush(Colors.LimeGreen);
                await Task.Delay(600);
                GameWinner.Foreground = new SolidColorBrush(Colors.Yellow);
                await Task.Delay(600);
                GameWinner.Foreground = new SolidColorBrush(Colors.SkyBlue);
                await Task.Delay(600);
                GameWinner.Foreground = new SolidColorBrush(Colors.Red);
                await Task.Delay(600);
            }
        }

        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PlotHitLocations(Bounces, PastGame, "GameSummary"));
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Splash());
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage(PlayerTwo, PlayerOne));
        }
    }
}
