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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        Player PlayerOne = null;
        Player PlayerTwo = null;

        public GamePage(Player pOne, Player pTwo)
        {
            InitializeComponent();
            PlayerOne = pOne;
            PlayerTwo = pTwo;
            
            //Player One DataContext
            PlayerOneUserName.DataContext = PlayerOne;
            PlayerOneName.DataContext = PlayerOne;
            PlayerOneNation.DataContext = PlayerOne;
            PlayerOnePPHand.DataContext = PlayerOne;
            
            //Player Two DataContext
            PlayerTwoUserName.DataContext = PlayerTwo;
            PlayerTwoName.DataContext = PlayerTwo;
            PlayerTwoNation.DataContext = PlayerTwo;
            PlayerTwoPPHand.DataContext = PlayerTwo;

        }
    }
}
