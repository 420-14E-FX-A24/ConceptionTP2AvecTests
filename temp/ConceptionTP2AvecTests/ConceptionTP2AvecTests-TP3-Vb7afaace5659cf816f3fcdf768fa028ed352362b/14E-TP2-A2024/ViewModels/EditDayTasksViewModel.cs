using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Automate.Interfaces;
using Automate.Utils;
using Automate.Views;

namespace Automate.ViewModels
{
	public class EditDayTasksViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		private readonly NavigationService _navigationService;
		private readonly MongoDBService _mongoService;
		private static IWindowService? _windowService;
		public IEnumerable<Models.Task.TypeEnum> EnumValues { get; } = Enum.GetValues(typeof(Models.Task.TypeEnum)).Cast<Models.Task.TypeEnum>();
		public Window Window { get; set; }

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

		private string _comment = string.Empty;
		public string Comment
		{
			get => _comment;
			set
			{
				_comment = value;
				OnPropertyChanged(nameof(Comment));
			}
		}

		private Models.Task.TypeEnum _selectedType;
		public Models.Task.TypeEnum SelectedType
		{
			get => _selectedType;
			set
			{
				if (_selectedType != value)
				{
					_selectedType = value;
					OnPropertyChanged(nameof(SelectedType));
				}
			}
		}

		private Models.Task? _selectedTask = null;
		public Models.Task? SelectedTask
		{
			get => _selectedTask;
			set
			{
				if (_selectedTask != value)
				{
					_selectedTask = value;
					IsTaskSelected = true;

					Comment = SelectedTask?.Comment ?? "";
					SelectedType = SelectedTask?.Type ?? Models.Task.TypeEnum.Semis;

					OnPropertyChanged(nameof(SelectedTask));
					OnPropertyChanged(nameof(IsTaskSelected));
					OnPropertyChanged(nameof(Comment));
					OnPropertyChanged(nameof(SelectedType));
				}
			}
		}
		public bool IsTaskSelected { get; set; } = false;

		private DateTime _dateSelection;
		public DateTime DateSelection
		{
			get => _dateSelection;
			set
			{
				if (_dateSelection != value)
				{
					_dateSelection = value;
					OnPropertyChanged(nameof(DateSelection));
				}
			}
		}

		public ICommand AddTaskCommand { get; }
		public ICommand EditTaskCommand { get; }
		public ICommand RemoveTaskCommand { get; }
		public ICommand ReturnHomeCommand { get; }


		public EditDayTasksViewModel(Window openedWindow)
		{
			if (openedWindow == null) throw new ArgumentNullException(nameof(openedWindow));

            _navigationService = new NavigationService();

            if (_windowService is null)
				_windowService = WindowServiceWrapper.GetInstance(this, openedWindow, _navigationService);

			if (_mongoService is null)
				_mongoService = new MongoDBService("AutomateDB");

			
			Window = openedWindow;

			AddTaskCommand = new RelayCommand(AddTask);
			EditTaskCommand = new RelayCommand(EditTask);
			RemoveTaskCommand = new RelayCommand(RemoveTask);
			ReturnHomeCommand = new RelayCommand(ReturnHome);

			SetTasks();
		}

		private void SetTasks()
		{
			Tasks = _windowService!.Tasks;
		}

		public void AddTask()
		{
			Models.Task task = new Models.Task(date: DateSelection);

			Tasks.Add(task);
			SelectedTask = task;
			SaveTask(task);

			Tasks = new ObservableCollection<Models.Task>(Tasks);
		}

		public void EditTask()
		{
			if (SelectedTask is null) return;

			SelectedTask.Type = SelectedType;
			SelectedTask.Comment = Comment;
			SaveTask(SelectedTask);

			Tasks = new ObservableCollection<Models.Task>(Tasks);
		}

		public void RemoveTask()
		{
			if (SelectedTask is null) return;

			_mongoService.RemoveTask(SelectedTask);
			Tasks.Remove(SelectedTask);

			Tasks = new ObservableCollection<Models.Task>(Tasks);
		}

		public void SaveTask(Models.Task task)
		{
			_mongoService.SaveTask(task);
		}

		public void ReturnHome()
		{
			HomeViewModel homeViewModel = new HomeViewModel(Window)
			{
				DateSelection = DateSelection
			};

			_navigationService.NavigateTo<HomeWindow>(homeViewModel, true);

			Window? editWindow = Application.Current.Windows
					  .Cast<Window>()
					  .FirstOrDefault(w => w.Name == "EditDayTasksView");

			if (editWindow is not null)
				editWindow.Close();
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
