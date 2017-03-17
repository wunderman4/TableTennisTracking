using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SelectPlayers.xaml
    /// </summary>
    public partial class SelectPlayers : Page
    {
        PlayerService ps = new PlayerService();
        List<Player> PlayerOneList;
        List<Player> PlayerTwoList;
        Player PlayerOne = null;
        Player PlayerTwo = null;

        public SelectPlayers()
        {
            InitializeComponent();
            GetPlayers();
        }

        // Gets the list of players
        private void GetPlayers()
        {
            // Gets list of players
            PlayerOneList = ps.ListPlayers();

            //copies list to player two
            PlayerTwoList = PlayerOneList;

            // assigns each list to the listbox on xaml page. 
            PlayerOneListBox.ItemsSource = PlayerOneList;
            PlayerTwoListBox.ItemsSource = PlayerTwoList;

        }

        // Player One Confirm
        private async void PlayerOneConfirm(object sender, RoutedEventArgs e)
        {
            // need to check if player two is null then
            // either set and wait or set and navigate. 
            foreach (Player p in PlayerOneListBox.SelectedItems)
            {
                if (p != PlayerTwo)
                {

                    PlayerOne = p;
                    if (PlayerTwo != null)
                    {
                        NavigationService.Navigate(new GamePage(PlayerOne, PlayerTwo));
                    }
                }
                else
                {
                    PlayerOneErrorSnackbar.IsActive = true;
                    await Task.Delay(2000);
                    PlayerOneErrorSnackbar.IsActive = false;
                }
            }

            

        }



        // Player Two Confrim
        private async void PlayerTwoConfirm(object sender, RoutedEventArgs e)
        {
            foreach (Player p in PlayerTwoListBox.SelectedItems)
            {
                if (p != PlayerOne)
                {
                    PlayerTwo = p;

                    if (PlayerOne != null)
                    {
                        NavigationService.Navigate(new GamePage(PlayerOne, PlayerTwo));
                    }
                }
                else
                {
                    PlayerTwoErrorSnackbar.IsActive = true;
                    await Task.Delay(2000);
                    PlayerTwoErrorSnackbar.IsActive = false;

                }
            }

            
        }

        private async void Cancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

    }
}
