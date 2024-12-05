using Automate.ViewModels;
using System.Windows;

namespace Automate.Views
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = new HomeViewModel(this);
        }
    }
}
