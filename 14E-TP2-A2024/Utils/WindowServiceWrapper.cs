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
using SharpCompress.Common;
using SharpCompress.Compressors.Xz;
using System.Diagnostics;


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
        public bool FileIsLoading { get; set; } = false;
        public string FileName { get; set; }
        private CancellationTokenSource _cancellationTokenSource;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WindowServiceWrapper(object viewModel = null, Window window = null, NavigationService navigationService = null, IWindowService windowService = null)
        {
            _viewModel = viewModel;
            _window = window;
            _navigationService = navigationService;
            if (windowService != null)
                _sharedSingleton = windowService;
            else
                _sharedSingleton = (IWindowService)this;
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

        private int _temperatureControlleDe;
        public int TemperatureControlleDe
        {
            get => _temperatureControlleDe == 0 ? TemperatureIdeale.Item1 : _temperatureControlleA;
            set
            {
                if (_temperatureControlleDe != value)
                {
                    _temperatureControlleDe = value;
                    OnPropertyChanged(nameof(TemperatureControlleDe));
                }
            }
        }

        private int _temperatureControlleA;
        public int TemperatureControlleA
        {
            get => _temperatureControlleA == 0 ? TemperatureIdeale.Item2 : _temperatureControlleA;
            set
            {
                if (_temperatureControlleA != value)
                {
                    _temperatureControlleA = value;
                    OnPropertyChanged(nameof(TemperatureControlleA));
                }
            }
        }
        
        private int _luminositeControlleDe;
        public int LuminositeControlleDe
        {
            get => _luminositeControlleDe == 0 ? LuminositeIdeale.Item1 : _luminositeControlleA;
            set
            {
                if (_luminositeControlleDe != value)
                {
                    _luminositeControlleDe = value;
                    OnPropertyChanged(nameof(LuminositeControlleDe));
                }
            }
        }

        private int _luminositeControlleA;
        public int LuminositeControlleA
        {
            get => _luminositeControlleA == 0 ? LuminositeIdeale.Item2 : _luminositeControlleA;
            set
            {
                if (_luminositeControlleA != value)
                {
                    _luminositeControlleA = value;
                    OnPropertyChanged(nameof(LuminositeControlleA));
                }
            }
        }


        private int _humiditeControlleDe;
        public int HumiditeControlleDe
        {
            get => _humiditeControlleDe == 0 ? HumiditeIdeale.Item1 : _humiditeControlleDe;
            set
            {
                if (_humiditeControlleDe != value)
                {
                    _humiditeControlleDe = value;
                    OnPropertyChanged(nameof(HumiditeControlleDe));
                }
            }
        }

        private int _humiditeControlleA;
        public int HumiditeControlleA
        {
            get => _humiditeControlleA == 0 ? HumiditeIdeale.Item2 : _humiditeControlleA;
            set
            {
                if (_humiditeControlleA != value)
                {
                    _humiditeControlleA = value;
                    OnPropertyChanged(nameof(HumiditeControlleA));
                }
            }
        }

        private (int, int) _temperatureIdeale = (13, 29); //55 and 85 degrees Fahrenheit
        public (int, int) TemperatureIdeale { get => _temperatureIdeale;}
        private (int, int) _humiditeIdeale = (65, 85); //65 and 85% humidité
        public (int, int) HumiditeIdeale { get => _humiditeIdeale; }
        private (int, int) _luminositeIdeale = (17391, 26087); //17391, 26087 lux
        public (int, int) LuminositeIdeale { get => _luminositeIdeale; }



        public string GiveAdviceTomatoParameters(int minValue, int maxValue, ref string accesseurConseil,
            int accesseurReelle, string propertyName)
        {
            string[] parameters = new string[5] { "fenêtres", "chauffage", 
                "ventilateurs", "arrosage", "lumières" };
            string[] actions = new string[2] { "ouvrir", "fermer" };
            accesseurConseil = "Aucun ajustement nécessaire";
            if (propertyName == "Temperature")
            {
                if (accesseurReelle < minValue)
                    accesseurConseil = CreateAdviceString(actions[1], parameters[0], actions[0], 
                        parameters[1], actions[1], parameters[2]);
                else if (accesseurReelle > maxValue)
                    accesseurConseil = CreateAdviceString(actions[0], parameters[0], actions[1], 
                        parameters[1], actions[0], parameters[2], actions[0], parameters[3]);
            }
            else if (propertyName == "Humidite")
            {
                if (accesseurReelle < minValue)
                    accesseurConseil = CreateAdviceString(actions[1], parameters[0], actions[1], 
                        parameters[1], actions[1], parameters[2], actions[0], parameters[3]);
                else if (accesseurReelle > maxValue)
                    accesseurConseil = CreateAdviceString(actions[0], parameters[0], actions[0], 
                        parameters[1], actions[0], parameters[2], actions[1], parameters[3]);
            }
            else if (propertyName == "Luminosite")
            {
                if (accesseurReelle < minValue)
                    accesseurConseil = CreateAdviceString(actions[1], parameters[0], actions[0], parameters[4]);
                else if (accesseurReelle > maxValue)
                    accesseurConseil = CreateAdviceString(actions[0], parameters[0], actions[1], parameters[4]);
            }
            return accesseurConseil;
        }

        private string CreateAdviceString(params string[] items)
        {
            if (items.Length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items.Length; i += 2)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(items[i]).Append(" ").Append(items[i + 1]);
            }

            return sb.ToString();
        }


        private string _luminositeConseil;
        public string LuminositeConseil
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
        private string _temperatureConseil;
        public string TemperatureConseil
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
        private string _humiditeConseil;
        public string HumiditeConseil
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


        private int _temperatureReelle;
        public int TemperatureReelle { 
			get => _temperatureReelle;
			set
			{
                if (_temperatureReelle != value)
                {
                    _temperatureReelle = value;
                    OnPropertyChanged(nameof(TemperatureReelle));
                    string oldValueConseil = TemperatureConseil;
                    GiveAdviceTomatoParameters(TemperatureIdeale.Item1, 
                        TemperatureIdeale.Item2, 
                        ref _temperatureConseil, TemperatureReelle, "Temperature");
                    if(TemperatureConseil != oldValueConseil)
                        OnPropertyChanged(nameof(TemperatureConseil));
                }
            } 
		}

        private int _luminositeReelle;
        public int LuminositeReelle { 
            get => _luminositeReelle;
			set
			{
                if (_luminositeReelle != value)
                {
                    _luminositeReelle = value;
                    OnPropertyChanged(nameof(LuminositeReelle));
                    string oldValueConseil = LuminositeConseil;
                    GiveAdviceTomatoParameters(LuminositeIdeale.Item1,
                        LuminositeIdeale.Item2,
                        ref _luminositeConseil, LuminositeReelle, "Luminosite");
                    if (LuminositeConseil != oldValueConseil)
                        OnPropertyChanged(nameof(LuminositeConseil));
                }
            }
		}

        private int _humiditeReelle;
        public int HumiditeReelle { 
            get => _humiditeReelle;
			set
			{
                if(_humiditeReelle != value)
                {
                    _humiditeReelle = value;
                    OnPropertyChanged(nameof(HumiditeReelle));
                    string oldValueConseil = HumiditeConseil;
                    GiveAdviceTomatoParameters(HumiditeIdeale.Item1,
                        HumiditeIdeale.Item2,
                        ref _humiditeConseil, HumiditeReelle, "Humidite");
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

        public Stream? OpenRead(string path)
        {
            Stream? stream = null;
            if (!FileIsLoading)
            {
                FileIsLoading = true;
                if(_sharedSingleton is WindowServiceWrapper)
                    stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                else
                    stream = _sharedSingleton.OpenRead(path);
                return stream;
            }
            return stream;
        }

        public void LoadFile(string fileName)
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(binPath, fileName);
            var encoding = Encoding.GetEncoding("iso-8859-1");

            using (var fileStream = OpenRead(filePath))
            {
                if (fileStream is null)
                    return;

                using (var reader = new StreamReader(fileStream, encoding))
                {
                    var lines = new List<string>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                    var data = lines.Skip(1).Select(l => l.Split(',')).ToList();
                    for (int i = 0; i < data.Count; i++)
                    {
                        AddLineToFileData(i, data[i]);
                    }
                }
            }
            FileIsLoading = false;
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
            int? parameter;
            while (MeteoDataIsRead)
            {
                if (currentLineIndex == nbLines)
                    currentLineIndex = 0;
                currentLineData = FileData[currentLineIndex];
                for (int i = 1; i < currentLineData.Length; i++)
                {
                    parameter = ParseStringInt(currentLineData[i]);
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

        private int? ParseStringInt(string parameter)
        {
            if (int.TryParse(parameter, out int result))
                return result;
            return null;
        }

    }

    
}