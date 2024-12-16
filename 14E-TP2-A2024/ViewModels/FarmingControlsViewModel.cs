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

		public ICommand SaveCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ReadMeteoDataCommand { get; }
        public ICommand StopReadingMeteoDataCommand { get; }
        public ICommand ReturnToHomeCommand { get; }
        public ICommand ActionEclairageCommand { get; }
        public ICommand ActionFenetreCommand { get; }
        public ICommand ActionArrosageCommand { get; }
        public ICommand ActionChauffageCommand { get; }
        public ICommand ActionVentilateurCommand { get; }


        private bool _isActionVentilateur;
        private bool _isActionChauffage;
        private bool _isActionArrosage;
        private bool _isActionFenetre;
        private bool _isActionEclairage;

       

        public event PropertyChangedEventHandler? PropertyChanged;


        public FarmingControlsViewModel(Window openedWindow)
        {
            if (_mongoService is null)
                _mongoService = new MongoDBService("AutomateDB");

            _navigationService = new NavigationService();
            Window = openedWindow;
            if (_windowService is null)
                InitialiserWindowService();

            SaveCommand = new RelayCommand(SaveValueClimatiques);
            LogoutCommand = new RelayCommand(Logout);
            ReadMeteoDataCommand = new RelayCommand(ReadMeteoData);
            StopReadingMeteoDataCommand = new RelayCommand(StopReadingMeteoData);
            ReturnToHomeCommand = new RelayCommand(ReturnToHome);
            ActionEclairageCommand = new RelayCommand(ActionEclairage);
            ActionFenetreCommand = new RelayCommand(ActionFenetre);
            ActionArrosageCommand = new RelayCommand(ActionArrosage);
            ActionChauffageCommand = new RelayCommand(ActionChauffage);
            ActionVentilateurCommand = new RelayCommand(ActionVentilateur);

			if (openedWindow is not null)
			{
				GetClimateSystems();
				GetClimateConditions();
			}
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
            {
                notifyPropertyChanged.PropertyChanged += WindowServiceWrapper_PropertyChanged;
            }
        }

        private void ActionVentilateur(object obj)
        {
            IsActionVentilateur = !IsActionVentilateur;
        }

        private void ActionChauffage(object obj)
        {
            IsActionChauffage = !IsActionChauffage;
        }

        private void ActionArrosage(object obj)
        {
            IsActionArrosage = !IsActionArrosage;
        }

        private void ActionFenetre(object obj)
        {
            IsActionFenetre = !IsActionFenetre;
        }

        private void ActionEclairage(object obj)
        {
            IsActionEclairage = !IsActionEclairage;
        }

        private void SaveValueClimatiques(object obj)
        {
            throw new NotImplementedException();
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

        public bool IsActionVentilateur
        {
            get => _isActionVentilateur;
            set
            {
                if (_isActionVentilateur != value)
                {
                    _isActionVentilateur = value;
                    OnPropertyChanged(nameof(IsActionVentilateur));
                }
            }
        }

        public bool IsActionChauffage
        {
            get => _isActionChauffage;
            set
            {
                if (_isActionChauffage != value)
                {
                    _isActionChauffage = value;
                    OnPropertyChanged(nameof(IsActionChauffage));
                }
            }
        }

        public bool IsActionEclairage
        {
            get => _isActionEclairage;
            set
            {
                if (_isActionEclairage != value)
                {
                    _isActionEclairage = value;
                    OnPropertyChanged(nameof(IsActionEclairage));
                }
            }
        }

        public bool IsActionArrosage
        {
            get => _isActionArrosage;
            set
            {
                if (_isActionArrosage != value)
                {
                    _isActionArrosage = value;
                    OnPropertyChanged(nameof(IsActionArrosage));
                }
            }
        }


        public bool IsActionFenetre
        {
            get => _isActionFenetre;
            set
            {
                if (_isActionFenetre != value)
                {
                    _isActionFenetre = value;
                    OnPropertyChanged(nameof(IsActionFenetre));
                }
            }
        }


		private ObservableCollection<ClimateSystem> _climateSystems;
		public ObservableCollection<ClimateSystem> ClimateSystems
		{
			get => _climateSystems;
			set
			{
				_climateSystems = value;
				OnPropertyChanged(nameof(ClimateSystems));
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

			if (systems == null || systems.Count == 0)
			{
				ClimateSystems = new ObservableCollection<ClimateSystem>
				{
					new ClimateSystem(ClimateSystem.SystemTypes.Light),
					new ClimateSystem(ClimateSystem.SystemTypes.Windows),
					new ClimateSystem(ClimateSystem.SystemTypes.Watering),
					new ClimateSystem(ClimateSystem.SystemTypes.Fan),
					new ClimateSystem(ClimateSystem.SystemTypes.Heat)
				};

				foreach (var climateSystem in ClimateSystems)
				{
					SaveClimateSystem(climateSystem);
				}
			}
			else
				ClimateSystems = systems;
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

				foreach (var climateCondition in ClimateConditions)
				{
					SaveClimateCondition(climateCondition);
				}
			}
			else
				ClimateConditions = conditions;
		}

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            string fullPropertyName = propertyName;

            if (propertyName == "TemperatureReelle"
                || propertyName == "HumiditeReelle"
                || propertyName == "LuminositeReelle"
                || propertyName == "TemperatureConseil"
                || propertyName == "HumiditeConseil"
                || propertyName == "LuminositeConseil"
                || propertyName == "HumiditeControlleA"
                || propertyName == "HumiditeControlleDe"
                || propertyName == "LuminositeControlleA"
                || propertyName == "LuminositeControlleDe"
                || propertyName == "TemperatureControlleA"
                || propertyName == "TemperatureControlleDe")
                fullPropertyName = "WindowService." + propertyName;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(fullPropertyName));
        }

		public void SaveClimateCondition(ClimateCondition climateCondition)
		{
			_mongoService.SaveClimateCondition(climateCondition);
		}

        private void WindowServiceWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}
