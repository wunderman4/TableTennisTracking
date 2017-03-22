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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Page
    {
        PlayerService ps = new PlayerService();
        List<Player> PlayerList;
        Player Player = null;

        public Leaderboard()
        {
            InitializeComponent();
            GetPlayers();
        }

        // Gets the list of players
        private void GetPlayers()
        {
            // Gets list of players
            PlayerList = ps.ListPlayers();

            // assigns each list to the listbox on xaml page. 
            PlayerListBox.ItemsSource = PlayerList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        
        // Hides all elemnts to wipe board for elements we want to show. 
        private void HideAll()
        {
            // Header, Col 0 Row 0 
            LeaderboardTitle.Visibility = Visibility.Collapsed;
            SelectPlayerTitle.Visibility = Visibility.Collapsed;

            // Left Card, Col 0 Row 1
            StatsByPlayer.Visibility = Visibility.Collapsed;
            PlayerListBox.Visibility = Visibility.Collapsed;
            GlobalButton.Visibility = Visibility.Collapsed;
            PVPButton.Visibility = Visibility.Collapsed;
            HitLocationButton.Visibility = Visibility.Collapsed;
            ReturnButton.Visibility = Visibility.Collapsed;
            HomeButton.Visibility = Visibility.Collapsed;

            // Header, Col 1, Row 0

            // Right Card, Col 1, Row 1
            StatsByPlayerConfirm.Visibility = Visibility.Collapsed;

        }

        private void ShowDefault()
        {
            LeaderboardTitle.Visibility = Visibility.Visible;
            StatsByPlayer.Visibility = Visibility.Visible;
            GlobalButton.Visibility = Visibility.Visible;
            PVPButton.Visibility = Visibility.Visible;
            HitLocationButton.Visibility = Visibility.Visible;
            HomeButton.Visibility = Visibility.Visible;
            

        }

        private void StatsByPlayer_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            SelectPlayerTitle.Visibility = Visibility.Visible;
            PlayerListBox.Visibility = Visibility.Visible;
            ReturnButton.Visibility = Visibility.Visible;
            StatsByPlayerConfirm.Visibility = Visibility.Visible;

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            ShowDefault();
            
        }

        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        private void GlobalButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void PVPButton_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            SelectPlayerTitle.Visibility = Visibility.Visible;
            PlayerListBox.Visibility = Visibility.Visible;
            ReturnButton.Visibility = Visibility.Visible;
        }

        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
