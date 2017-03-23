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
        // Player Player = null;

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

            // assigns each list to the listboxes on xaml page. 
            PlayerListBox.ItemsSource = PlayerList;
            PVPSelectOne.ItemsSource = PlayerList;
            PVPSelectTwo.ItemsSource = PlayerList;
        }

        
        // Hides all elemnts to wipe board for elements we want to show. 
        private void HideAll()
        {
            // Header, Col 0 Row 0 
            LeaderboardTitle.Visibility = Visibility.Collapsed;
            SelectPlayerTitle.Visibility = Visibility.Collapsed;
            PVPReturnButton.Visibility = Visibility.Collapsed;

            // Left Card, Col 0 Row 1
            StatsByPlayer.Visibility = Visibility.Collapsed;
            PlayerListBox.Visibility = Visibility.Collapsed;
            GlobalButton.Visibility = Visibility.Collapsed;
            PVPButton.Visibility = Visibility.Collapsed;
            HitLocationButton.Visibility = Visibility.Collapsed;
            ReturnButton.Visibility = Visibility.Collapsed;
            HomeButton.Visibility = Visibility.Collapsed;
            PVPSelectOne.Visibility = Visibility.Collapsed;
            PVPOneConfirmButton.Visibility = Visibility.Collapsed;
            PVPOneBinding.Visibility = Visibility.Collapsed;
            PVPOneSelectNewButton.Visibility = Visibility.Collapsed;

            // Header, Col 1, Row 0

            // Right Card, Col 1, Row 1
            StatsByPlayerBinding.Visibility = Visibility.Collapsed;
            StatsByPlayerConfirm.Visibility = Visibility.Collapsed;
            PVPSelectTwo.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            PVPTwoBinding.Visibility = Visibility.Collapsed;

        }

        // Returns the page to the default view.
        private void ShowDefault()
        {
            LeaderboardTitle.Visibility = Visibility.Visible;
            StatsByPlayer.Visibility = Visibility.Visible;
            GlobalButton.Visibility = Visibility.Visible;
            PVPButton.Visibility = Visibility.Visible;
            HitLocationButton.Visibility = Visibility.Visible;
            HomeButton.Visibility = Visibility.Visible;
            

        }

        // Changes the page to show stats for a specific player
        private async void StatsByPlayer_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            HideAll();
            SelectPlayerTitle.Visibility = Visibility.Visible;
            PlayerListBox.Visibility = Visibility.Visible;
            ReturnButton.Visibility = Visibility.Visible;
            StatsByPlayerConfirm.Visibility = Visibility.Visible;
            StatsByPlayerBinding.Visibility = Visibility.Hidden;

        }

        // Resets page to default View
        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            HideAll();
            ShowDefault();
            
        }

        // Navigates to splash page
        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        // Shows global Stats
        private void GlobalButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Shows player v player stats
        private void PVPButton_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            PVPReturnButton.Visibility = Visibility.Visible;
            PVPSelectOne.Visibility = Visibility.Visible;
            PVPSelectTwo.Visibility = Visibility.Visible;
            PVPOneConfirmButton.Visibility = Visibility.Visible;
            PVPTwoConfirmButton.Visibility = Visibility.Visible;
        }

        // Shows hit location stats by game
        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StatsByPlayerConfirm_Click(object sender, RoutedEventArgs e)
        {
            StatsByPlayerBinding.DataContext = PlayerListBox.SelectedItem;
            StatsByPlayerBinding.Visibility = Visibility.Visible;
        }

        private void PVPOneConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PVPOneBinding.DataContext = PVPSelectOne.SelectedItem;
            PVPOneBinding.Visibility = Visibility.Visible;
            PVPSelectOne.Visibility = Visibility.Collapsed;
            PVPOneConfirmButton.Visibility = Visibility.Collapsed;
            PVPOneSelectNewButton.Visibility = Visibility.Visible;
            
        }

        private void PVPTwoConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PVPTwoBinding.DataContext = PVPSelectTwo.SelectedItem;
            PVPTwoBinding.Visibility = Visibility.Visible;
            PVPSelectTwo.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            
        }

        private void PVPOneSelectNewButton_Click(object sender, RoutedEventArgs e)
        {
            PVPOneSelectNewButton.Visibility = Visibility.Collapsed;
            PVPOneBinding.Visibility = Visibility.Collapsed;
            PVPSelectOne.Visibility = Visibility.Visible;
            PVPOneConfirmButton.Visibility = Visibility.Visible;
        }
    }
}
