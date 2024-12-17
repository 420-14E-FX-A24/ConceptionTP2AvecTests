using Automate.ViewModels;
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
using System.Windows.Shapes;

namespace Automate.Views
{
    /// <summary>
    /// Logique d'interaction pour FarmingWindow.xaml
    /// </summary>
    public partial class FarmingWindow : Window
    {
        public FarmingWindow()
        {
            InitializeComponent();
            DataContext = new FarmingControlsViewModel(this);
       
        }
    }
}
