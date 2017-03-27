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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Page
    {

        BackgroundWorker workerOne;
        PlayerService ps = new PlayerService();
        List<Player> PlayerList;

        

        public Leaderboard()
        {
            InitializeComponent();
            GetPlayers();
            workerOne = new BackgroundWorker();
            workerOne.DoWork += new DoWorkEventHandler(workerOne_DoWork);
        }

        private void workerOne_DoWork(object sender, DoWorkEventArgs e)
        {
            // Gather Stats and assign data context
            Player PVPOne = (Player)PVPSelectOne.SelectedItem;
            PlayerGameStats PVPOneStats = new PlayerGameStats(PVPOne.Id);
            PVPOneUserNameBinding.DataContext = PVPOne;
            PVPOneBinding.DataContext = PVPOneStats;
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
            PVPOneArea.Visibility = Visibility.Collapsed;

            // Header, Col 1, Row 0
            Statistics.Visibility = Visibility.Collapsed;
            SelectPlayerTwoTitle.Visibility = Visibility.Collapsed;
            PVPTitle.Visibility = Visibility.Collapsed;


            // Right Card, Col 1, Row 1
            StatsByPlayerBinding.Visibility = Visibility.Collapsed;
            StatsByPlayerConfirm.Visibility = Visibility.Collapsed;
            PVPSelectTwo.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            PVPTwoBinding.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            PVPTwoSelectNewButton.Visibility = Visibility.Collapsed;
            PVPTwoArea.Visibility = Visibility.Collapsed;

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
            Statistics.Visibility = Visibility.Visible;

        }

        // Changes the page to show stats for a specific player
        private async void StatsByPlayer_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            HideAll();
            SelectPlayerTitle.Visibility = Visibility.Visible;
            PlayerListBox.Visibility = Visibility.Visible;
            ReturnButton.Visibility = Visibility.Visible;
            Statistics.Visibility = Visibility.Visible;
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
            PVPTitle.Visibility = Visibility.Visible;
        }

        // Shows hit location stats by game
        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Confirms selection of single player and displays stats
        private void StatsByPlayerConfirm_Click(object sender, RoutedEventArgs e)
        {
            // collapse 
            Statistics.Visibility = Visibility.Visible;

            // Gather Stats by Selected Player and sets data context
            Player SinglePlayer = (Player)PlayerListBox.SelectedItem;
            PlayerGameStats SinglePlayerStats = new PlayerGameStats(SinglePlayer.Id);
            SBPUserNameBinding.DataContext = SinglePlayer;
            StatsByPlayerBinding.DataContext = SinglePlayerStats;

            // Shows correct elements 
            StatsByPlayerBinding.Visibility = Visibility.Visible;
        }

        // Confirms Selection of first player for PVP
        private async void PVPOneConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (PVPSelectOne.SelectedItem != null)
            {
                // Collapse
                PVPSelectOne.Visibility = Visibility.Collapsed;
                PVPOneConfirmButton.Visibility = Visibility.Collapsed;

                workerOne.RunWorkerAsync();
                //// Gather Stats and assign data context
                //Player PVPOne = (Player)PVPSelectOne.SelectedItem;
                //PlayerGameStats PVPOneStats = new PlayerGameStats(PVPOne.Id);
                //PVPOneUserNameBinding.DataContext = PVPOne;
                //PVPOneBinding.DataContext = PVPOneStats;

                // Shows Correct Elements
                PVPOneBinding.Visibility = Visibility.Visible;

                PVPOneSelectNewButton.Visibility = Visibility.Visible;
                PVPOneArea.Visibility = Visibility.Visible;
                
            }
            else
            {
                NoPlayerSelected.IsActive = true;
                await Task.Delay(3000);
                NoPlayerSelected.IsActive = false;
            }
        }

        // Confrims Selection of second player for pvp
        private async void PVPTwoConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (PVPSelectTwo.SelectedItem != null)
            {
                // Collapse
                PVPSelectTwo.Visibility = Visibility.Collapsed;
                PVPTwoConfirmButton.Visibility = Visibility.Collapsed;

                // Gather Stats and assign data context
                Player PVPTwo = (Player)PVPSelectTwo.SelectedItem;
                PlayerGameStats PVPTwoStats = new PlayerGameStats(PVPTwo.Id);
                PVPTwoUserNameBinding.DataContext = PVPTwo;
                PVPTwoBinding.DataContext = PVPTwoStats;

                // Shows Correct Elements
                PVPTwoBinding.Visibility = Visibility.Visible;

                PVPTwoSelectNewButton.Visibility = Visibility.Visible;
                PVPTwoArea.Visibility = Visibility.Visible;
            }
            else
            {
                NoPlayerSelected.IsActive = true;
                await Task.Delay(3000);
                NoPlayerSelected.IsActive = false;
            }

        }

        // Reopens the select player menu to select different player one
        private void PVPOneSelectNewButton_Click(object sender, RoutedEventArgs e)
        {
            PVPOneSelectNewButton.Visibility = Visibility.Collapsed;
            PVPOneBinding.Visibility = Visibility.Collapsed;
            PVPOneArea.Visibility = Visibility.Collapsed;
            PVPSelectOne.Visibility = Visibility.Visible;
            PVPOneConfirmButton.Visibility = Visibility.Visible;
        }
        
        // Reopens the slect player menu to select different player two
        private void PVPTwoSelectNewButton_Click(object sender, RoutedEventArgs e)
        {
            PVPTwoSelectNewButton.Visibility = Visibility.Collapsed;
            PVPTwoBinding.Visibility = Visibility.Collapsed;
            PVPTwoArea.Visibility = Visibility.Collapsed;
            PVPSelectTwo.Visibility = Visibility.Visible;
            PVPTwoConfirmButton.Visibility = Visibility.Visible;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
            PBarOne.Visibility = Visibility.Visible;

        }

        private void StopWork(object sender, DoWorkEventArgs e)
        {
            PBarOne.Visibility = Visibility.Collapsed;
        }

    }
}
