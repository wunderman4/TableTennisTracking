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

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for NewPlayer.xaml
    /// </summary>
    public partial class NewPlayer : Page
    {
        //public User TestUser;



        public NewPlayer()
        {
            InitializeComponent();
        }

        private async void AddCancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
        }

        private void Submit(object sender, RoutedEventArgs e)
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

            using (var db = new TableTennisTrackerDb())
            {
                db.Players.Add(newPlayer);
                db.SaveChanges();
            }
        }
    }
}



//TestUser = new User
//{
//    Name = NameTextBox.Text,
//    UserName = UserNameTextBox.Text
//};