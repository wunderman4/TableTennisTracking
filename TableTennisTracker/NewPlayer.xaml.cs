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
    /// Interaction logic for NewPlayer.xaml
    /// </summary>
    public partial class NewPlayer : Page
    {
        //public User TestUser;
        PlayerService ps = new PlayerService();



        public NewPlayer()
        {
            InitializeComponent();
        }

        private async void AddCancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        private async void Submit(object sender, RoutedEventArgs e)
        {
            int Age = Convert.ToInt32(AgeTextBox.Text);

            Player newPlayer = new Player
            {
                UserName = UserNameTextBox.Text,
                PlayerName = NameTextBox.Text,
                Age = Age,
                Nationality = CountryTextBox.Text,
                HandPreference = PPH.Text
            };

            ps.AddPlayer(newPlayer);

            await Task.Delay(250);
            NavigationService.Navigate(new Splash());

        }
    }
}



//TestUser = new User
//{
//    Name = NameTextBox.Text,
//    UserName = UserNameTextBox.Text
//};