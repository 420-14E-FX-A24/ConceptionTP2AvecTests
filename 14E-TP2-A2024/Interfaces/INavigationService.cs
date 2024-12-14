using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automate.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<T>(object dataContext = null, bool isAdmin = false) where T : Window, new();
        void Close(Window window);
        void CloseWindow(string windowName);

    }
}
