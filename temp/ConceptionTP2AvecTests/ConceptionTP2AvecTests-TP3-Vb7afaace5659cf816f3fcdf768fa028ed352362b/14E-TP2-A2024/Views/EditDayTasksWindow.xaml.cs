using Automate.ViewModels;
using System.Windows;

namespace Automate.Views
{
	public partial class EditDayTasksWindow : Window
	{
		public EditDayTasksWindow()
		{
			InitializeComponent();
			DataContext = new EditDayTasksViewModel(this);
		}
	}
}
