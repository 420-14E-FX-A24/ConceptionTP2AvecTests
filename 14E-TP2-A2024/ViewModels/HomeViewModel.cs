using Automate.Utils;
using Automate.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Automate.Interfaces;
using System.Data;

namespace Automate.ViewModels
{
	public class HomeViewModel : INotifyPropertyChanged
	{
		public int ArrosagesAlerts { get; set; }
		public int SemisAlerts { get; set; }
		public Window Window { get; set; }

		public ICommand ShowDayTasksCommand { get; }
		public ICommand EditDayTasksCommand { get; }
		public ICommand CloseDialogCommand { get; }
		public ICommand ControleCommand { get; }
		public RelayCommand ShowDialogCommand { get; }
		public RelayCommand LogoutCommand { get; }
		public RelayCommand GetUnitValuesCommand { get; }

		public event PropertyChangedEventHandler? PropertyChanged;
		private readonly NavigationService _navigationService;
		private readonly MongoDBService _mongoService;
		private static IWindowService? _windowService;

		public HomeViewModel(Window openedWindow)
		{
			if (_mongoService is null)
				_mongoService = new MongoDBService("AutomateDB");

			_navigationService = new NavigationService();

			if (_windowService is null)
				_windowService = WindowServiceWrapper.GetInstance(this, openedWindow, _navigationService);

			ShowDayTasksCommand = new RelayCommand(ShowDayTasks);
			EditDayTasksCommand = new RelayCommand(EditDayTasks);
			ShowDialogCommand = new RelayCommand(ShowDialog);
			CloseDialogCommand = new RelayCommand(CloseDialog);
			ControleCommand = new RelayCommand(NaviguerControle);
			LogoutCommand = new RelayCommand(Logout);
			GetUnitValuesCommand = new RelayCommand(GetUnitValues);

			Window = openedWindow;

			if (openedWindow is not null)
				ShowDayTasks();
		}

		private bool _isAdmin;
		public bool IsAdmin
		{
			get => _isAdmin;
			set
			{
				if (_isAdmin != value)
				{
					_isAdmin = value;
					_windowService.IsAdmin = value;
				}
				OnPropertyChanged(nameof(IsAdmin));
			}
		}

		private DateTime _dateSelection = DateTime.Now;
		public DateTime DateSelection
		{
			get => _dateSelection;
			set
			{
				if (_dateSelection != value)
				{
					_dateSelection = value;

					ShowDayTasks();
					OnPropertyChanged(nameof(DateSelection));
				}
			}
		}

		private ObservableCollection<Models.Task> _tasks;
		public ObservableCollection<Models.Task> Tasks
		{
			get => _tasks;
			set
			{
				_tasks = value;
				OnPropertyChanged(nameof(Tasks));
			}
		}

		private Models.Task? _task;
		public Models.Task? Task
		{
			get => _task;
			set
			{
				_task = value;
				OnPropertyChanged(nameof(Task));
			}
		}

		public ObservableCollection<Models.Task> GetTasksFromDate(DateTime date)
		{
			DateTime startOfDay = date.Date;
			DateTime endOfDay = startOfDay.AddDays(1).AddSeconds(-1);

			var filteredTasks = Tasks
				.Where(t => t.Date >= startOfDay && t.Date <= endOfDay)
				.ToList();

			return new ObservableCollection<Models.Task>(filteredTasks);
		}

		public void ShowDayTasks()
		{
			Tasks = _mongoService.GetMonthTasks(DateSelection);

			ShowAlerts();

			OnPropertyChanged(nameof(Tasks));
		}

		public void ShowAlerts()
		{
			if (DateSelection.Date != DateTime.Now.Date) { return; }

			ObservableCollection<Models.Task> tasks = _mongoService.GetTasks(DateTime.Now);

			ArrosagesAlerts = tasks.Count(t => t.Type == Models.Task.TypeEnum.Arrosage);
			SemisAlerts = tasks.Count(t => t.Type == Models.Task.TypeEnum.Semis);

			if (ArrosagesAlerts > 0 || SemisAlerts > 0)
				ShowDialog();
		}

		public void EditDayTasks()
		{
			EditDayTasksViewModel editViewModel = new EditDayTasksViewModel(Window)
			{
				Tasks = GetTasksFromDate(DateSelection),
				DateSelection = DateSelection
			};

			_navigationService.NavigateTo<EditDayTasksWindow>(editViewModel);

			Window? homeWindow = Application.Current.Windows
					  .Cast<Window>()
					  .FirstOrDefault(w => w.Name == "HomeView");

			if (homeWindow is not null)
				homeWindow.Close();
		}

		public void ShowDialog()
		{
			var dialogContent = new AlertDialog();
			try
			{
				Application.Current.Dispatcher.Invoke(async () =>
				{
					if (Window is not null)
					{
						await System.Threading.Tasks.Task.Delay(100);
						DialogHost.Show(dialogContent, "Alertes");
					}
				});
			}
			catch (InvalidOperationException ex)
			{
				Debug.WriteLine($"InvalidOperationException11: {ex.Message}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception: {ex.Message}");
			}
		}

		public void CloseDialog()
		{
			DialogHost.Close("Alertes", null);
		}

		private void NaviguerControle(object obj)
		{
			if (IsAdmin)
			{
				_navigationService.NavigateTo<FarmingWindow>(null, IsAdmin);
				_navigationService.Close(Window);
			}

		}

		public void Logout()
		{
			IsAdmin = false;
			Window.DataContext = null;
			_windowService = null;

			_navigationService.NavigateTo<LoginWindow>();

			foreach (Window window in Application.Current.Windows)
			{
				if (window.Name == "EditDayTasksView" || window.Name == "HomeView")
				{
					window.Close();
					break;
				}
			}
		}

		public void GetUnitValues()
		{

		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
