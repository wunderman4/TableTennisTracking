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

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for SelectPlayers.xaml
    /// </summary>
    public partial class SelectPlayers : Page
    {
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
            using (var db = new TableTennisTrackerDb())
            {
                PlayerOneList = (from p in db.Players
                                 select p).ToList();
                PlayerTwoList = PlayerOneList;
            }
            PlayerOneListBox.ItemsSource = PlayerOneList;
            PlayerTwoListBox.ItemsSource = PlayerTwoList;

        }

        // Player One Confirm
        private void PlayerOneConfirm(object sender, RoutedEventArgs e)
        {
            // need to check if player two is null then
            // either set and wait or set and navigate. 
            foreach (Player p in PlayerOneListBox.SelectedItems)
            {
                if (PlayerOne == null)
                {

                    PlayerOne = p;

                }
            }

            if (PlayerTwo != null)
            {
                NavigationService.Navigate(new GamePage(PlayerOne,PlayerTwo));
            }

        }

        

        // Player Two Confrim
        private void PlayerTwoConfirm(object sender, RoutedEventArgs e)
        {
            foreach (Player p in PlayerTwoListBox.SelectedItems)
            {
                if (PlayerTwo == null)
                {

                    PlayerTwo = p;

                }
            }

            if (PlayerOne != null)
            {
                NavigationService.Navigate(new GamePage(PlayerOne, PlayerTwo));
            }
        }

        private async void Cancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

    }
}
