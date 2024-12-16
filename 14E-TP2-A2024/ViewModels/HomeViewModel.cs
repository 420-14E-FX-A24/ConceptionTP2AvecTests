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
        public ICommand ReadMeteoDataCommand { get; }
        public ICommand StopReadingMeteoDataCommand { get; }
        public RelayCommand ShowDialogCommand { get; }
		public RelayCommand LogoutCommand { get; }

		public event PropertyChangedEventHandler? PropertyChanged;

        private void WindowServiceWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Notify that a property has changed
            OnPropertyChanged(e.PropertyName);
        }

        private readonly NavigationService _navigationService;
		private readonly MongoDBService _mongoService;
		private static IWindowService? _windowService;

        public HomeViewModel(Window openedWindow)
		{
			if (_mongoService is null)
				_mongoService = new MongoDBService("AutomateDB");

            _navigationService = new NavigationService();
            Window = openedWindow;
            if (_windowService is null)
				InitialiserWindowService();

            ShowDayTasksCommand = new RelayCommand(ShowDayTasks);
			EditDayTasksCommand = new RelayCommand(EditDayTasks);
			ShowDialogCommand = new RelayCommand(ShowDialog);
			CloseDialogCommand = new RelayCommand(CloseDialog);
			ControleCommand = new RelayCommand(NaviguerControle);
            LogoutCommand = new RelayCommand(Logout);
            ReadMeteoDataCommand = new RelayCommand(ReadMeteoData);
            StopReadingMeteoDataCommand = new RelayCommand(StopReadingMeteoData);

            if (openedWindow is not null)
				ShowDayTasks();
		}

        private void ReadMeteoData()
        {
            WindowService.FileName = "TempData(corrige).txt";
            if (!WindowService.MeteoDataIsRead)
            {
                WindowService.MeteoDataIsRead = true;
                WindowService.ReadMeteoData();
            }
            else
                MessageBox.Show("Lecture des données en cours. " +
                    "Appuyer sur le bouton Arrêter la lecture. " +
                    "Ensuite, vous pourrez réessayer.",
                    "Lecture de la météo.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StopReadingMeteoData()
        {
            WindowService.MeteoDataIsRead = false;
        }

        private void InitialiserWindowService()
		{
            _windowService = WindowServiceWrapper.GetInstance(this, Window, _navigationService);
            if (_windowService is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += WindowServiceWrapper_PropertyChanged;
            }
        }

        private void NaviguerControle(object obj)
        {
			if (IsAdmin)
			{
                _navigationService.NavigateTo<FarmingWindow>(null, IsAdmin);
                _navigationService.Close(Window);
            }
				
        }

        public WindowServiceWrapper WindowService
        {
            get => (WindowServiceWrapper)_windowService;
            set
            {
                if (_windowService != value)
                {
                    _windowService = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAdmin;
		public bool IsAdmin
		{
			get => WindowService.IsAdmin;
			set
			{
				if (_isAdmin != value)
				{
					_isAdmin = value;
					WindowService.IsAdmin = value;
				}
				OnPropertyChanged(nameof(WindowService.IsAdmin));
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
                if (Application.Current != null)
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
                else
                {
                    Debug.WriteLine("Application.Current is null.");
                }
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

        private void Logout()
        {
            if (_windowService is not null)
                _windowService.Logout();
        }
        

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			string fullPropertyName = propertyName;

            if (propertyName == "TemperatureReelle" 
				|| propertyName == "HumiditeReelle" 
				|| propertyName == "LuminositeReelle")
                fullPropertyName = "WindowService." + propertyName;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(fullPropertyName));
        }
	}
}
