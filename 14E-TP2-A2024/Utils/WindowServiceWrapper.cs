using Automate.Interfaces;
using Automate.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;

namespace Automate.Utils
{
	public class WindowServiceWrapper : IWindowService
	{
		private static IWindowService? _sharedSingleton;
		private object _viewModel;
		private Window _window;
        private NavigationService _navigationService;

        private WindowServiceWrapper(object viewModel, Window window, NavigationService navigationService)
		{
			_viewModel = viewModel;
			_window = window;
            _navigationService = navigationService;
        }

		public static IWindowService GetInstance(object viewModel, Window window, NavigationService navigationService)
		{
			if (_sharedSingleton is null)
			{
				_sharedSingleton = new WindowServiceWrapper(viewModel, window, navigationService);
			}

			return _sharedSingleton;
		}

		public DateTime DateSelection
		{
			get => (_viewModel as IWindowService)?.DateSelection ?? DateTime.MinValue;
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.DateSelection = value;
				}
			}
		}

		public bool IsAdmin
		{
			get => (_viewModel as IWindowService)?.IsAdmin ?? false;
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.IsAdmin = value;
				}
			}
		}

        public NavigationService NavigationService
        {
            get => (_navigationService);
        }

        public ObservableCollection<Task> Tasks
		{
			get => (_viewModel as IWindowService)?.Tasks ?? new ObservableCollection<Task>();
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.Tasks = value;
				}
			}
		}

		//public ObservableCollection<ClimateSystem> ClimateSystems
		//{
		//	get => (_viewModel as IWindowService)?.ClimateSystems ?? new ObservableCollection<ClimateSystem>();
		//	set
		//	{
		//		if (_viewModel is IWindowService service)
		//		{
		//			service.ClimateSystems = value;
		//		}
		//	}
		//}

		//public ObservableCollection<ClimateCondition> ClimateConditions
		//{
		//	get => (_viewModel as IWindowService)?.ClimateConditions ?? new ObservableCollection<ClimateCondition>();
		//	set
		//	{
		//		if (_viewModel is IWindowService service)
		//		{
		//			service.ClimateConditions = value;
		//		}
		//	}
		//}

		public void Close()
		{
			_window.Close();
		}

        public void Logout()
        {
            IsAdmin = false;
            _window.DataContext = null;
            NavigationService.NavigateTo<LoginWindow>();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Name == "EditDayTasksView" || window.Name == "HomeView")
                {
                    window.Close();
                    break;
                }
            }
            _sharedSingleton = null;
        }
    }
}