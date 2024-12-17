using Automate.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Automate
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? passwordBox = sender as PasswordBox;
			if (DataContext is LoginViewModel viewModel && passwordBox is not null)
			{
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
