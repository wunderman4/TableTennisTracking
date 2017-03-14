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
        Player PlayerOne = null;
        public SelectPlayers()
        {
            InitializeComponent();
            GetPlayers();
        }

        private void GetPlayers()
        {
            using (var db = new TableTennisTrackerDb())
            {
                PlayerOneList = (from p in db.Players
                                 select p).ToList();
            }
            selectPlayer.ItemsSource = PlayerOneList;

        }

        private async void Cancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        private async void PTwoConfirm(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new GamePage());
        }

        private void PlayerOneConfirm(object sender, RoutedEventArgs e)
        {
            

            foreach (Player p in PlayerOneList)
            {
                if (PlayerOne == null)
                {
                    if (p.IsSelected == true)
                    {
                        PlayerOne = p;
                    }
                }

                

            }
            
        }
    }
}
