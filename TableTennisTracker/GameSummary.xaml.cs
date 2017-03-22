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
    /// Interaction logic for GameSummary.xaml
    /// </summary>
    public partial class GameSummary : Page
    {
        Game PastGame = null;

        public GameSummary(Game _game)
        {
            InitializeComponent();
            PastGame = _game;
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
