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
        
        PlayerService ps = new PlayerService();
        List<Player> PlayerList;

        

        public Leaderboard()
        {
            InitializeComponent();
            GetPlayers();
            //workerOne = new BackgroundWorker();
            //workerOne.DoWork += new DoWorkEventHandler(workerOne_DoWork);
            ProgressAnimation();
            
        }

        //.................................... Page Load Methods...........................................
        
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

        // Shows then Collpases Progess bar on Page Load
        private async void ProgressAnimation()
        {
            await Task.Delay(2500);
            PBarOne.Visibility = Visibility.Collapsed;
        }

        //..................................... Show Default & Hide All Elemets...................................

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
            //PVPOneBinding.Visibility = Visibility.Collapsed;
            PVPOneSelectNewButton.Visibility = Visibility.Collapsed;
            PVPOneArea.Visibility = Visibility.Collapsed;

            // Header, Col 1, Row 0
            Welcome.Visibility = Visibility.Collapsed;
            SelectPlayerTwoTitle.Visibility = Visibility.Collapsed;
            PVPTitle.Visibility = Visibility.Collapsed;
            ColOneStatsByPlayerTitle.Visibility = Visibility.Collapsed;
            GlobalLeadersTitle.Visibility = Visibility.Collapsed;


            // Right Card, Col 1, Row 1
            StatsByPlayerBinding.Visibility = Visibility.Collapsed;
            StatsByPlayerConfirm.Visibility = Visibility.Collapsed;
            PVPSelectTwo.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            PVPTwoBinding.Visibility = Visibility.Collapsed;
            PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
            PVPTwoSelectNewButton.Visibility = Visibility.Collapsed;
            PVPTwoArea.Visibility = Visibility.Collapsed;
            GlobalStatsArea.Visibility = Visibility.Collapsed;


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
            Welcome.Visibility = Visibility.Visible;

        }

        //.......................................Default Page View Buttons.................................

        // Button to Show global Stats
        private async void GlobalButton_Click(object sender, RoutedEventArgs e)
        {
            PBarSpan.Visibility = Visibility.Visible;
            Task<GlobalGameStats> GetGlobalStats = Task.Factory.StartNew(() => GlobalStats());
            GlobalStatBinding.DataContext = await GetGlobalStats;

            // Collapse 
            PBarSpan.Visibility = Visibility.Collapsed;
            Welcome.Visibility = Visibility.Collapsed;
            // Show
            GlobalStatsArea.Visibility = Visibility.Visible;
            GlobalLeadersTitle.Visibility = Visibility.Visible;
            
        }

        // Button to show single player stats
        private async void StatsByPlayer_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            HideAll();
            SelectPlayerTitle.Visibility = Visibility.Visible;
            PlayerListBox.Visibility = Visibility.Visible;
            ReturnButton.Visibility = Visibility.Visible;
            StatsByPlayerConfirm.Visibility = Visibility.Visible;
            StatsByPlayerBinding.Visibility = Visibility.Hidden;
            ColOneStatsByPlayerTitle.Visibility = Visibility.Visible;

        }

        // Button to Show player v player stats
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

        // Button to Show hit location stats by game
        private void HitLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Button to Navigate to splash page
        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        //........................................Confrim Selection Buttons..................................

        // Confirms selection of single player and displays stats
        private async void StatsByPlayerConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerListBox.SelectedItem != null) {

                PBarSpan.Visibility = Visibility.Visible;
                // Gather Stats by Selected Player and sets data context
                Player SinglePlayer = (Player)PlayerListBox.SelectedItem;
                Task<PlayerGameStats> SingleStats = Task.Factory.StartNew(() => SingleGet(SinglePlayer.Id));
                SBPUserNameBinding.DataContext = SinglePlayer;
                StatsByPlayerBinding.DataContext = await SingleStats;

                // collapse 
                PBarSpan.Visibility = Visibility.Collapsed;
                
                // Shows correct elements 
                StatsByPlayerBinding.Visibility = Visibility.Visible;
                
            }
            else
            {
                NoPlayerSelected.IsActive = true;
                await Task.Delay(3000);
                NoPlayerSelected.IsActive = false;
            }
        }

        // Confirms Selection of first player for PVP
        private async void PVPOneConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (PVPSelectOne.SelectedItem != null)
            {
                PBarOne.Visibility = Visibility.Visible;
                // Gather Stats and assign data context
                Player PVPOne = (Player)PVPSelectOne.SelectedItem;
                Task<PlayerGameStats> OneStats = Task.Factory.StartNew(() => PVPOneGet(PVPOne.Id));
                PVPOneBinding.DataContext = await OneStats;
                PVPOneUserNameBinding.DataContext = PVPOne;

                // Collapse
                PVPSelectOne.Visibility = Visibility.Collapsed;
                PVPOneConfirmButton.Visibility = Visibility.Collapsed;
                PBarOne.Visibility = Visibility.Collapsed;
                // Shows Correct Elements
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

                PBarTwo.Visibility = Visibility.Visible;
                // Gather Stats and assign data context
                Player PVPTwo = (Player)PVPSelectTwo.SelectedItem;
                Task<PlayerGameStats> PVPTwoStats = Task.Factory.StartNew(()=> PVPTwoGet(PVPTwo.Id));
                PVPTwoUserNameBinding.DataContext = PVPTwo;
                PVPTwoBinding.DataContext = await PVPTwoStats;

                // Collapse
                PVPSelectTwo.Visibility = Visibility.Collapsed;
                PVPTwoConfirmButton.Visibility = Visibility.Collapsed;
                PBarTwo.Visibility = Visibility.Collapsed;

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

        //...................................... Loading Calls For Async Programming........................
        
        //Gloabal Stats Loading
        private GlobalGameStats GlobalStats()
        {
            GlobalGameStats GlobalGameStats = new GlobalGameStats();
            return GlobalGameStats;
        }

        private PlayerGameStats PVPOneGet(int id)
        {
            
            PlayerGameStats PVPOneStats = new PlayerGameStats(id);

            return PVPOneStats;
        }

        private PlayerGameStats PVPTwoGet(int id)
        {
            PlayerGameStats PVPTwoStats = new PlayerGameStats(id);

            return PVPTwoStats;
        }

        private PlayerGameStats SingleGet(int id)
        {
            PlayerGameStats SinglePlayerStats = new PlayerGameStats(id);
            return SinglePlayerStats;
        }

        //........................................ Return and Select Another Buttons..........................

        // Returns Button to reset page
        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            HideAll();
            ShowDefault();

        }

        // Reopens the select player menu to select different player one
        private void PVPOneSelectNewButton_Click(object sender, RoutedEventArgs e)
        {
            PVPOneSelectNewButton.Visibility = Visibility.Collapsed;
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


    }
}
