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
    /// Interaction logic for EditPlayer.xaml
    /// </summary>
    public partial class EditPlayer : Page
    {
        PlayerService ps = new PlayerService();
        Player Player = null;
        
        

        public EditPlayer(Player p)
        {
            InitializeComponent();
            Player = p;

            // sets page Data Context. 
            //this.DataContext = this;
            UserNameTextBox.DataContext = Player;
            NameTextBox.DataContext = Player;
            HeightFt.DataContext = Player.HeightFt.ToString();
            HeightIn.DataContext = Player.HeightInch.ToString();
            //HeightFt.DataContext = Player;
            //HeightIn.DataContext = Player;
            AgeTextBox.DataContext = Player;
            PPH.DataContext = Player;
            CountryTextBox.DataContext = Player;

            ShowHeightFt(Player.HeightFt);
            ShowHeightIn(Player.HeightInch);
            ShowHand(Player.HandPreference);
        }

        public void ShowHeightFt(int height)
        {
            switch (height)
            {
                case 4:
                    H4.IsSelected = true;
                    break;
                case 5:
                    H5.IsSelected = true;
                    break;
                case 6:
                    H6.IsSelected = true;
                    break;
                case 7:
                    H7.IsSelected = true;
                    break;
            }
        }

        public void ShowHeightIn(int height)
        {
            switch (height)
            {
                case 0:
                    I0.IsSelected = true;
                    break;
                case 1:
                    I1.IsSelected = true;
                    break;
                case 2:
                    I2.IsSelected = true;
                    break;
                case 3:
                    I3.IsSelected = true;
                    break;
                case 4:
                    I4.IsSelected = true;
                    break;
                case 5:
                    I5.IsSelected = true;
                    break;
                case 6:
                    I6.IsSelected = true;
                    break;
                case 7:
                    I7.IsSelected = true;
                    break;
                case 8:
                    I8.IsSelected = true;
                    break;
                case 9:
                    I9.IsSelected = true;
                    break;
                case 10:
                    I10.IsSelected = true;
                    break;
                case 11:
                    I11.IsSelected = true;
                    break;
            }
        }

        public void ShowHand(string hand)
        {
            switch (hand)
            {
                case "RIGHTY":
                    RIGHTY.IsSelected = true;
                    break;
                case "SOUTH PAW":
                    SOUTHPAW.IsSelected = true;
                    break;
                case "AMBIDEXTROUS":
                    AMBIDEXTROUS.IsSelected = true;
                    break;
            }
        }

        private async void CancelEdit(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new ManagePlayers());
        }

        // Checks required fields, converts data type, Assigns to type User then adds to database.
        private async void Submit(object sender, RoutedEventArgs e)
        {
            // Checks required fields
            if (AgeTextBox.Text == "" ||
                UserNameTextBox.Text == "" ||
                NameTextBox.Text == "" ||
                HeightFt.Text == "" ||
                HeightIn.Text == "" ||
                PPH.Text == "" ||
                CountryTextBox.Text == "")
            {
                // SnackBar Popup if Feilds not filled in.
                EnterAllFieldsError.IsActive = true;
                await Task.Delay(3000);
                EnterAllFieldsError.IsActive = false;
            }
            else
            {

                // Converts DataTypes 
                int age = Convert.ToInt32(AgeTextBox.Text);
                int heightFeet = Convert.ToInt32(HeightFt.Text.ToString());
                int heightInches = Convert.ToInt32(HeightIn.Text.ToString());

                // Updates Player
                Player.UserName = UserNameTextBox.Text;
                Player.PlayerName = NameTextBox.Text;
                Player.Age = age;
                Player.HeightFt = heightFeet;
                Player.HeightInch = heightInches;
                Player.Nationality = CountryTextBox.Text;
                Player.HandPreference = PPH.Text;

                // Updates Database
                ps.UpdatePlayer(Player);

                // Navigation Back to Manage Players Page
                await Task.Delay(200);
                NavigationService.Navigate(new ManagePlayers());
            }
        }
    }
}
