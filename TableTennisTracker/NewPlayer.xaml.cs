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
        //Add Services to page
        PlayerService ps = new PlayerService();
        
        public NewPlayer()
        {
            InitializeComponent();
        }

        // Navigates Home
        private async void AddCancel(object sender, RoutedEventArgs e)
        {
            await Task.Delay(350);
            NavigationService.Navigate(new Splash());
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
                await Task.Delay(2000);
                EnterAllFieldsError.IsActive = false;
            }
            else
            {

                // Converts DataTypes 
                int age = Convert.ToInt32(AgeTextBox.Text);
                int heightFeet = Convert.ToInt32(HeightFt.Text.ToString());
                int heightInches = Convert.ToInt32(HeightIn.Text.ToString());
                // Creates New Player
                Player newPlayer = new Player
                {
                    UserName = UserNameTextBox.Text,
                    PlayerName = NameTextBox.Text,
                    Age = age,
                    HeightFt = heightFeet,
                    HeightInch = heightInches,
                    Nationality = CountryTextBox.Text,
                    HandPreference = PPH.Text

                };

                ps.AddPlayer(newPlayer);

                await Task.Delay(200);
                NavigationService.Navigate(new Splash());
            }
        }
    }
}