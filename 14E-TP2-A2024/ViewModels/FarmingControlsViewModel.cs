using Automate.Interfaces;
using Automate.Models;
using Automate.Utils;
using Automate.Views;
using Microsoft.Xaml.Behaviors;
using ModernWpf.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Automate.ViewModels
{
    class FarmingControlsViewModel : INotifyPropertyChanged
    {

        public Window Window { get; set; }
        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;
        private static IWindowService? _windowService;
        private readonly int[] DefaultDayMinRangeValues = new int[] { 20, 40000, 60 };
		private readonly int[] DefaultDayMaxRangeValues = new int[] { 28, 70000, 85 };
        private readonly int[] DefaultNightMinRangeValues = new int[] { 15, 0, 65 };
		private readonly int[] DefaultNightMaxRangeValues = new int[] { 18, 5000, 75 };
		public ClimateCondition TemperatureCondition { get; set; }
		public ClimateCondition HumidityCondition { get; set; }
		public ClimateCondition LuminosityCondition { get; set; }

		public ICommand SaveCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ReadMeteoDataCommand { get; }
        public ICommand StopReadingMeteoDataCommand { get; }
        public ICommand ReturnToHomeCommand { get; }
        public ICommand SystemActionCommand { get; }

		public ObservableCollection<string> Modes { get; } = new ObservableCollection<string>
		{
			"Jour", "Nuit"
		};

		private string _selectedMode;
		public string SelectedMode
		{
			get => _selectedMode;
			set
			{
				if (_selectedMode != value)
				{
					_selectedMode = value;
					OnPropertyChanged(nameof(SelectedMode));
					LoadClimateConditions();
				}
			}
		}

		private void LoadClimateConditions()
		{
			bool isDay = SelectedMode == "Jour";

			TemperatureCondition = _climateConditions.FirstOrDefault(c => c.IsDay == isDay && c.Type == ClimateCondition.ConditionTypes.Temperature);
			HumidityCondition = _climateConditions.FirstOrDefault(c => c.IsDay == isDay && c.Type == ClimateCondition.ConditionTypes.Humidity);
			LuminosityCondition = _climateConditions.FirstOrDefault(c => c.IsDay == isDay && c.Type == ClimateCondition.ConditionTypes.Luminosity);

			OnPropertyChanged(nameof(TemperatureCondition));
			OnPropertyChanged(nameof(HumidityCondition));
			OnPropertyChanged(nameof(LuminosityCondition));
		}

		public event PropertyChangedEventHandler? PropertyChanged;


        public FarmingControlsViewModel(Window openedWindow)
        {
            if (_mongoService is null)
                _mongoService = new MongoDBService("AutomateDB");

            _navigationService = new NavigationService();
            Window = openedWindow;
            if (_windowService is null)
                InitialiserWindowService();
            else
            {
                if (_windowService is WindowServiceWrapper windowServiceWrapper)
                    windowServiceWrapper.ViewModel = this;
            }
                
            SaveCommand = new RelayCommand(SaveClimateConditions);
            LogoutCommand = new RelayCommand(Logout);
            ReadMeteoDataCommand = new RelayCommand(ReadMeteoData);
            StopReadingMeteoDataCommand = new RelayCommand(StopReadingMeteoData);
            ReturnToHomeCommand = new RelayCommand(ReturnToHome);
            SystemActionCommand = new RelayCommand(SystemAction);

			if (openedWindow is not null)
			{
				GetClimateSystems();
				GetClimateConditions();
			}

			SelectedMode = "Jour";
		}

        

        private void ReturnToHome(object obj)
        {
            _navigationService.NavigateTo<HomeWindow>(null, WindowService.IsAdmin);
            _navigationService.Close(Window);
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
                notifyPropertyChanged.PropertyChanged += WindowServiceWrapper_PropertyChanged;
            if (_windowService is WindowServiceWrapper windowServiceWrapper)
                windowServiceWrapper.ViewModel = this;
        }

        private void SystemAction(object obj)
        {
            if (obj is ClimateSystem.SystemTypes type) 
            { 
                switch (type)
                {
                    case ClimateSystem.SystemTypes.Fan:
                        FanSystem.IsActivated = !FanSystem.IsActivated;
						SaveClimateSystem(FanSystem);
						break;
                    case ClimateSystem.SystemTypes.Light:
					    LightSystem.IsActivated = !LightSystem.IsActivated;
						SaveClimateSystem(LightSystem);
						break;
				    case ClimateSystem.SystemTypes.Heat:
					    HeatSystem.IsActivated = !HeatSystem.IsActivated;
						SaveClimateSystem(HeatSystem);
						break;
				    case ClimateSystem.SystemTypes.Windows:
					    WindowsSystem.IsActivated = !WindowsSystem.IsActivated;
						SaveClimateSystem(WindowsSystem);
						break;
				    case ClimateSystem.SystemTypes.Watering:
					    WateringSystem.IsActivated = !WateringSystem.IsActivated;
						SaveClimateSystem(WateringSystem);
						break;
			    }
			}
		}

        private void Logout()
        {
            if(_windowService is not null)
                _windowService.Logout();
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

        private ClimateSystem _fanSystem;
        public ClimateSystem FanSystem
        {
            get => _fanSystem;
            set
            {
                if (_fanSystem != value)
                {
					_fanSystem = value;
                    OnPropertyChanged(nameof(FanSystem));
                }
            }
        }

        private ClimateSystem _heatSystem;
        public ClimateSystem HeatSystem
        {
            get => _heatSystem;
            set
            {
                if (_heatSystem != value)
                {
					_heatSystem = value;
                    OnPropertyChanged(nameof(HeatSystem));
                }
            }
        }

        private ClimateSystem _lightSystem;
        public ClimateSystem LightSystem
        {
            get => _lightSystem;
            set
            {
                if (_lightSystem != value)
                {
					_lightSystem = value;
                    OnPropertyChanged(nameof(LightSystem));
                }
            }
        }

        private ClimateSystem _wateringSystem;
        public ClimateSystem WateringSystem
        {
            get => _wateringSystem;
            set
            {
                if (_wateringSystem != value)
                {
					_wateringSystem = value;
                    OnPropertyChanged(nameof(WateringSystem));
                }
            }
        }

        private ClimateSystem _windowsSystem;
        public ClimateSystem WindowsSystem
        {
            get => _windowsSystem;
            set
            {
                if (_windowsSystem != value)
                {
					_windowsSystem = value;
                    OnPropertyChanged(nameof(WindowsSystem));
                }
            }
        }

		private ObservableCollection<ClimateCondition> _climateConditions;
		public ObservableCollection<ClimateCondition> ClimateConditions
		{
			get => _climateConditions;
			set
			{
				_climateConditions = value;
				OnPropertyChanged(nameof(ClimateConditions));
			}
		}

		public void GetClimateSystems()
		{
			var systems = _mongoService.GetClimateSystems();

			foreach (var type in Enum.GetValues<ClimateSystem.SystemTypes>())
			{
				var system = systems.FirstOrDefault(s => s.Type == type) ?? new ClimateSystem(type);

				switch (type)
				{
					case ClimateSystem.SystemTypes.Light:
						LightSystem = system;
						break;
					case ClimateSystem.SystemTypes.Windows:
						WindowsSystem = system;
						break;
					case ClimateSystem.SystemTypes.Watering:
						WateringSystem = system;
						break;
					case ClimateSystem.SystemTypes.Fan:
						FanSystem = system;
						break;
					case ClimateSystem.SystemTypes.Heat:
						HeatSystem = system;
						break;
				}

                if (!systems.Any(s => s.Type == type))
                    SaveClimateSystem(system);
			}
		}

		public void SaveClimateSystem(ClimateSystem climateSystem)
		{
			_mongoService.SaveClimateSystem(climateSystem);
		}

		public void GetClimateConditions()
		{
			var conditions = _mongoService.GetClimateConditions();

			if (conditions == null || conditions.Count == 0)
			{
				ClimateConditions = new ObservableCollection<ClimateCondition>
				{
					new ClimateCondition(ClimateCondition.ConditionTypes.Temperature, DefaultDayMinRangeValues[0], DefaultDayMaxRangeValues[0], true),
					new ClimateCondition(ClimateCondition.ConditionTypes.Temperature, DefaultNightMinRangeValues[0], DefaultNightMaxRangeValues[0], false),
					new ClimateCondition(ClimateCondition.ConditionTypes.Luminosity, DefaultDayMinRangeValues[1], DefaultDayMaxRangeValues[1], true),
					new ClimateCondition(ClimateCondition.ConditionTypes.Luminosity, DefaultNightMinRangeValues[1], DefaultNightMaxRangeValues[1], false),
					new ClimateCondition(ClimateCondition.ConditionTypes.Humidity, DefaultDayMinRangeValues[2], DefaultDayMaxRangeValues[2], true),
					new ClimateCondition(ClimateCondition.ConditionTypes.Humidity, DefaultNightMinRangeValues[2], DefaultNightMaxRangeValues[2], false)
				};

                SaveClimateConditions();
			}
			else
				ClimateConditions = conditions;
		}

		public void SaveClimateConditions()
		{
			foreach (var climateCondition in ClimateConditions)
			{
				_mongoService.SaveClimateCondition(climateCondition);
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			string fullPropertyName = propertyName;

			if (propertyName == "TemperatureReelle"
			 || propertyName == "HumiditeReelle"
			 || propertyName == "LuminositeReelle"
			 || propertyName == "TemperatureConseil"
			 || propertyName == "HumiditeConseil"
			 || propertyName == "LuminositeConseil")
				fullPropertyName = "WindowService." + propertyName;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(fullPropertyName));
		}

        /// <summary>
        /// Permet de notifier le view model qu'une propriété a changée. 
        /// Dans le cas complexe que l'accesseur appartient à un groupe de méthodes, sa nouvelle valeur est passée par 
        /// WindowServiceWrapper. L'accesseur n'aura pas besoin d'être déplacé pour recevoir sa nouvelle valeur dynamique. 
        /// Il pourra demeurer dans le view model. Reflection est utilisée pour mettre à jour l'accesseur selon son type.
        /// </summary>
        /// <param name="sender">Source de l'évènement.</param>
        /// <param name="e">PropertyChangedEventArgs, il contient la donnée.</param>
        private void WindowServiceWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e is CustomPropertyChangedEvent ex)
            {
                var newValue = ex.NewValue;
                if (newValue is not null)
                {
                    var propertyType = GetPropertyType(e.PropertyName);
                    if (propertyType != null && propertyType.IsInstanceOfType(newValue))
                    {
                        var property = GetType().GetProperty(e.PropertyName);
                        if (property != null)
                            property.SetValue(this, newValue);
                    }
                }
            }
            OnPropertyChanged(e.PropertyName);
        }

        private Type GetPropertyType(string propertyName)
        {
            var property = this.GetType().GetProperty(propertyName);
            return property?.PropertyType;
        }
    }
}
