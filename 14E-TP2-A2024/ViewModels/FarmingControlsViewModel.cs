using Automate.Interfaces;
using Automate.Models;
using Automate.Utils;
using Microsoft.Xaml.Behaviors;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public ICommand ActionEclairageCommand { get; }
        public ICommand ActionFenetreCommand { get; }
        public ICommand ActionArrosageCommand { get; }
        public ICommand ActionChauffageCommand { get; }
        public ICommand ActionVentilateurCommand { get; }

        private float _temperatureControlleDe;
        private float _luminositeControlleDe;
        private float _humiditeControlleDe;
        private float _temperatureControlleA;
        private float _luminositeControlleA;
        private float _humiditeControlleA;
        private float _temperatureReelle;
        private float _luminositeReelle;
        private float _humiditeReelle;

        private bool _isActionVentilateur;
        private bool _isActionChauffage;
        private bool _isActionArrosage;
        private bool _isActionFenetre;
        private bool _isActionEclairage;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FarmingControlsViewModel(Window openedWindow)
        {
            _navigationService = new NavigationService();
            if (_mongoService is null)
                _mongoService = new MongoDBService("AutomateDB");

            if (_windowService is null)
                _windowService = WindowServiceWrapper.GetInstance(this, openedWindow, _navigationService);

            Window = openedWindow;

            SaveCommand = new RelayCommand(SaveValueClimatiques);
            LogoutCommand = new RelayCommand(Logout);
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


        public float TemperatureControlleDe
        {
            get => _temperatureControlleDe;
            set
            {
                if (_temperatureControlleDe != value)
                {
                    _temperatureControlleDe = value;
                    OnPropertyChanged(nameof(TemperatureControlleDe));
                }
            }
        }

        public float TemperatureControlleA
        {
            get => _temperatureControlleA;
            set
            {
                if (_temperatureControlleA != value)
                {
                    _temperatureControlleA = value;
                    OnPropertyChanged(nameof(TemperatureControlleA));
                }
            }
        }

        public float TemperatureReelle
        {
            get => _temperatureReelle;
            set
            {
                if (_temperatureReelle != value)
                {
                    _temperatureReelle = value;
                    OnPropertyChanged(nameof(TemperatureReelle));
                }
            }
        }

        public float LuminositeControlleDe
        {
            get => _luminositeControlleDe;
            set
            {
                if (_luminositeControlleDe != value)
                {
                    _luminositeControlleDe = value;
                    OnPropertyChanged(nameof(LuminositeControlleDe));
                }
            }
        }

        public float LuminositeControlleA
        {
            get => _luminositeControlleA;
            set
            {
                if (_luminositeControlleA != value)
                {
                    _luminositeControlleA = value;
                    OnPropertyChanged(nameof(LuminositeControlleA));
                }
            }
        }

        public float LuminositeReelle
        {
            get => _luminositeReelle;
            set
            {
                if (_luminositeReelle != value)
                {
                    _luminositeReelle = value;
                    OnPropertyChanged(nameof(LuminositeReelle));
                }
            }
        }

        public float HumiditeControlleDe
        {
            get => _humiditeControlleDe;
            set
            {
                if (_humiditeControlleDe != value)
                {
                    _humiditeControlleDe = value;
                    OnPropertyChanged(nameof(HumiditeControlleDe));
                }
            }
        }

        public float HumiditeControlleA
        {
            get => _humiditeControlleA;
            set
            {
                if (_humiditeControlleA != value)
                {
                    _humiditeControlleA = value;
                    OnPropertyChanged(nameof(HumiditeControlleA));
                }
            }
        }

        public float HumiditeReelle
        {
            get => _humiditeReelle;
            set
            {
                if (_humiditeReelle != value)
                {
                    _humiditeReelle = value;
                    OnPropertyChanged(nameof(HumiditeReelle));
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

		public void SaveClimateCondition(ClimateCondition climateCondition)
		{
			_mongoService.SaveClimateCondition(climateCondition);
		}

	}
}
