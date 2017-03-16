using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
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
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        Player PlayerOne = null;
        Player PlayerTwo = null;
        Game CurrentGame = null;
        int _playerOneScore;
        int _playerTwoScore;
        SoundPlayer PointScored = new SoundPlayer(@"../../Sounds/PointScored.wav");
        SoundPlayer BallSet = new SoundPlayer(@"../../Sounds/BallSet.wav");
        SoundPlayer BadServe = new SoundPlayer(@"../../Sounds/BadServe.wav");

        // Constructor
        public GamePage(Player pOne, Player pTwo)
        {
            InitializeComponent();
            PlayerOne = pOne;
            PlayerTwo = pTwo;
            NewGame();
            this.DataContext = this;
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

        // Creates the current game object
        private void NewGame()
        {
            CurrentGame = new Game
            {
                Player1 = PlayerOne,
                Player2 = PlayerTwo,
                Player1Score = PlayerOneScore,
                Player2Score = PlayerTwoScore
            };
        }

        // Add Point button Player One
        private void PlayerOneAddPoint_Click(object sender, RoutedEventArgs e)
        {
            PlayerOneScore++;
            PlayScore();  

        }

        //Subtract Point Button Player One
        private void PlayerOneSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerOneScore !=0)
            {
                PlayerOneScore--;
                PlayBadServe();
            }
            
        }

        // Add Point Button Player Two
        private void PlayerTwoAddPoint_Click(object sender, RoutedEventArgs e)
        {
            PlayerTwoScore++;
            PlayScore();
        }

        // Subtract Point Player Two
        private void PlayerTwoSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerTwoScore != 0)
            {
                PlayerTwoScore--;
                PlayBadServe();
            }
            
        }


        // Player One Score and Game Board Update 
        public int PlayerOneScore
        {
            get {return _playerOneScore; }
            set
            {
                if (_playerOneScore != value)
                {
                    _playerOneScore = value;
                    CurrentGame.Player1Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PlayerOneScore"));
                    }
                }
            }
        }
        // Player Two Score and Game Board Update
        public int PlayerTwoScore
        {
            get {return _playerTwoScore; }
            set
            {
                if (_playerTwoScore != value)
                {
                    _playerTwoScore = value;
                    CurrentGame.Player2Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PlayerTwoScore"));
                    }
                }
            }
        }

        // Plays Point Scored Sound
        private void PlayScore()
        {
            PointScored.Load(); 
            PointScored.Play();
        }
        // Plays Ball Set Sound
        private void PlayBallSet()
        {
            BallSet.Load();
            BallSet.Play();
        }
        // Plays Bad Serve Sound
        private void PlayBadServe()
        {
            BadServe.Load();
            BadServe.Play();
        }

        private void PlayerOneRecord_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
