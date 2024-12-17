using Automate.Interfaces;
using Automate.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading.Tasks;
using System.Text;
using System.Threading;


namespace Automate.Utils
{
    public class WindowServiceWrapper : IWindowService, INotifyPropertyChanged
    {
        private static IWindowService? _sharedSingleton;
        private object _viewModel;
        private Window _window;
        private NavigationService _navigationService;
        public Dictionary<int, string[]> FileData { get; set; } = new Dictionary<int, string[]>();
        public int MeteoChangeDelay { get; set; } = 10;

        public string FileName { get; set; }
        private CancellationTokenSource _cancellationTokenSource;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public NavigationService NavigationService
        {
            get => (_navigationService);
        }

        public object ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    OnPropertyChanged();
                }

            }
        }
        private DateTime _dateSelection;
        public DateTime DateSelection
        {
            get
            {
                if (_viewModel is IWindowService service)
                {
                    return service.DateSelection;
                }
                return _dateSelection;
            }
            set
            {
                if (_viewModel is IWindowService service)
                {
                    service.DateSelection = value;
                }
                else
                {
                    if (_dateSelection != value)
                    {
                        _dateSelection = value;
                        OnPropertyChanged(nameof(IsAdmin));
                    }
                }
            }
        }


        private bool _isAdmin;
        public bool IsAdmin
        {
            get
            {
                if (_viewModel is IWindowService service)
                {
                    return service.IsAdmin;
                }
                return _isAdmin;
            }
            set
            {
                if (_viewModel is IWindowService service)
                {
                    service.IsAdmin = value;
                }
                else
                {
                    if (_isAdmin != value)
                    {
                        _isAdmin = value;
                        OnPropertyChanged(nameof(IsAdmin));
                    }
                }
            }
        }



        private ObservableCollection<Automate.Models.Task> _tasks;

        public ObservableCollection<Automate.Models.Task> Tasks
        {
            get
            {
                if (_viewModel is IWindowService service)
                {
                    return service.Tasks;
                }
                return _tasks ??= new ObservableCollection<Automate.Models.Task>();
            }
            set
            {
                if (_viewModel is IWindowService service)
                {
                    service.Tasks = value;
                }
                else
                {
                    if (_tasks != value)
                    {
                        _tasks = value;
                        OnPropertyChanged(nameof(Tasks));
                    }
                }
            }
        }



        private (int, int) _temperatureIdeale = (13, 29); //55 and 85 degrees Fahrenheit
        public (int, int) TemperatureIdeale { get => _temperatureIdeale;}
        private (int, int) _humiditeIdeale = (65, 85); //65 and 85% humidité
        public (int, int) HumiditeIdeale { get => _humiditeIdeale; }
        private (int, int) _luminositeIdeale = (17391, 26087); //17391, 26087 lux
        public (int, int) LuminositeIdeale { get => _luminositeIdeale; }
        private void GiveAdviceTomatoParameters(int minValue, int maxValue, ref float accesseurConseil, float accesseurReelle)
        {

            if (accesseurReelle < minValue)
                accesseurConseil = minValue;
            else if (accesseurReelle > maxValue)
                accesseurConseil = maxValue;
            else
                accesseurConseil = accesseurReelle;
        }

        private float _luminositeConseil;
        public float LuminositeConseil
        {
            get => _luminositeConseil;
            set
            {
                if (_luminositeConseil != value)
                {
                    _luminositeConseil = value;
                    OnPropertyChanged(nameof(LuminositeConseil));
                }
            }
        }
        private float _temperatureConseil;
        public float TemperatureConseil
        {
            get => _temperatureConseil;
            set
            {
                if (_temperatureConseil != value)
                {
                    _temperatureConseil = value;
                    OnPropertyChanged(nameof(TemperatureConseil));
                }
            }
        }
        private float _humiditeConseil;
        public float HumiditeConseil
        {
            get => _humiditeConseil;
            set
            {
                if (_humiditeConseil != value)
                {
                    _humiditeConseil = value;
                    OnPropertyChanged(nameof(HumiditeConseil));
                }
            }
        }


        private float _temperatureReelle;
        public float TemperatureReelle { 
			get => _temperatureReelle;
			set
			{
                if (_humiditeReelle != value)
                {
                    _temperatureReelle = value;
                    OnPropertyChanged(nameof(TemperatureReelle));
                    float oldValueConseil = TemperatureConseil;
                    GiveAdviceTomatoParameters(TemperatureIdeale.Item1, 
                        TemperatureIdeale.Item2, 
                        ref _temperatureConseil, TemperatureReelle);
                    if(TemperatureConseil != oldValueConseil)
                        OnPropertyChanged(nameof(TemperatureConseil));
                }
            } 
		}

        private float _luminositeReelle;
        public float LuminositeReelle { 
            get => _luminositeReelle;
			set
			{
                if (_humiditeReelle != value)
                {
                    _luminositeReelle = value;
                    OnPropertyChanged(nameof(LuminositeReelle));
                    float oldValueConseil = LuminositeConseil;
                    GiveAdviceTomatoParameters(LuminositeIdeale.Item1,
                        LuminositeIdeale.Item2,
                        ref _luminositeConseil, LuminositeReelle);
                    if (LuminositeConseil != oldValueConseil)
                        OnPropertyChanged(nameof(LuminositeConseil));
                }
            }
		}

        private float _humiditeReelle;
        public float HumiditeReelle { 
            get => _humiditeReelle;
			set
			{
                if(_humiditeReelle != value)
                {
                    _humiditeReelle = value;
                    OnPropertyChanged(nameof(HumiditeReelle));
                    float oldValueConseil = HumiditeConseil;
                    GiveAdviceTomatoParameters(HumiditeIdeale.Item1,
                        HumiditeIdeale.Item2,
                        ref _humiditeConseil, HumiditeReelle);
                    if (HumiditeConseil != oldValueConseil)
                        OnPropertyChanged(nameof(HumiditeConseil));
                }    
            }
		}


        private bool _meteoDataIsRead;
        public bool MeteoDataIsRead
        {
            get => _meteoDataIsRead;
            set
            {
                if (_meteoDataIsRead != value)
                {
                    _meteoDataIsRead = value;
                    OnPropertyChanged(nameof(MeteoDataIsRead));
                    if (!_meteoDataIsRead)
                        _cancellationTokenSource?.Cancel();
                }
            }
        }

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


        public void LoadFile(string fileName)
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(binPath, fileName);
            var encoding = Encoding.GetEncoding("iso-8859-1");

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, encoding))
            {
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                var data = lines.Skip(1).Select(l => l.Split(',')).ToList();
                var headers = lines.First().Split(',');

                for (int i = 0; i < data.Count; i++)
                {
                    AddLineToFileData(i, data[i]);
                }
            }


        }

        public void AddLineToFileData(int index, string[] line)
        {
            if (!FileData.ContainsKey(index))
                FileData.Add(index, line);
        }

        public void ReadMeteoData()
        {
            try
            {
                if (FileData.Count == 0)
                    LoadFile(FileName);
                if (FileData.Count > 0)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    DisplayLineDelay(MeteoChangeDelay, _cancellationTokenSource.Token);
                }
                    
            }
            catch (Exception ex) 
            {
                throw new Exception("La lecture du fichier à échoué. Format ou données invalide.", ex);
            }
            
        }

        async private void DisplayLineDelay(int delay, CancellationToken cancellationToken)
        {
            int nbLines = FileData.Count;
            int currentLineIndex = 0;
            string[] currentLineData;
            float? parameter;
            while (MeteoDataIsRead)
            {
                if (currentLineIndex == nbLines)
                    currentLineIndex = 0;
                currentLineData = FileData[currentLineIndex];
                for (int i = 1; i < currentLineData.Length; i++)
                {
                    parameter = ParseStringFloat(currentLineData[i]);
                    if (parameter.HasValue)
                    {
                        if (i == 1)
                            TemperatureReelle = parameter.Value;
                        if (i == 2)
                            HumiditeReelle = parameter.Value;
                        if (i == 3)
                            LuminositeReelle = parameter.Value;
                    }
                }
                currentLineIndex++;
                try
                {
                    await System.Threading.Tasks.Task.Delay(delay * 1000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
          
            }
        }

        private float? ParseStringFloat(string parameter)
        {
            float result;
            if (float.TryParse(parameter, out result))
                return result;
            else if (int.TryParse(parameter, out int intResult))
            {
                result = intResult;
                return result;
            }
            return null;
        }

      
    }

    
}