using Automate.Interfaces;
using Automate.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace Automate.Utils
{
    public class NavigationService: INavigationService
    {
        public void NavigateTo<T>(object dataContext = null, bool isAdmin = false) where T : Window, new()
        {
            var window = new T();
            if (dataContext != null)
                window.DataContext = dataContext;
            window.Show();
           
            if (window.DataContext is Automate.ViewModels.HomeViewModel)
                ((Automate.ViewModels.HomeViewModel)window.DataContext).IsAdmin = isAdmin;
        }

        public void Close(Window window)
        {
            if(window is not null)
                window.Close();
            else
            {
                Debug.WriteLine("window a été nulle");
            }
               
        }

        public void CloseWindow(string windowName)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Name == windowName)
                {
                    window.Close();
                    break;
                }
            }
        }
    }

}
