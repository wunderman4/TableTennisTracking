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

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for SelectPlayers.xaml
    /// </summary>
    public partial class SelectPlayers : Page
    {
        public SelectPlayers()
        {
            InitializeComponent();
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
    }
}
