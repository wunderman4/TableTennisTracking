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
    /// Interaction logic for ManagePlayers.xaml
    /// </summary>
    public partial class ManagePlayers : Page
    {
        PlayerService ps = new PlayerService();
        List<Player> PlayerList;
        Player Player = null;

        public ManagePlayers()
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

        // Edit Player
        private async void EditPlayer(object sender, RoutedEventArgs e)
        {
             
            foreach (Player p in PlayerListBox.SelectedItems)
            {
                    Player = p;
            }
            if (Player != null)
            {
                NavigationService.Navigate(new EditPlayer(Player));
            }
            else
            {
                NoPlayerSelected.IsActive = true;
                await Task.Delay(2000);
                NoPlayerSelected.IsActive = false;
            }
            

        }

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {

            foreach (Player p in PlayerListBox.SelectedItems)
            {
                Player = p;
            }

            if (Player != null)
            {
                ConfirmDelete.Visibility = Visibility.Visible;
                await Task.Delay(4000);
                ConfirmDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoPlayerSelected.IsActive = true;
                await Task.Delay(2000);
                NoPlayerSelected.IsActive = false;
            }
        }

        private async void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Player != null)
            {
                if (ps.PlayerHasGames(Player.Id))
                {
                    CantDelete.IsActive = true;
                    await Task.Delay(4000);
                    CantDelete.IsActive = false;
                }
                else
                {
                    ps.DeletePlayer(Player.Id);
                    NavigationService.Navigate(new ManagePlayers());
                    
                }
                
            }
            
        }
    }
}
