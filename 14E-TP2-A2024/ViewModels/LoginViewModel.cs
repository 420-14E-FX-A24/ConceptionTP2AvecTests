using Automate.Interfaces;
using Automate.Utils;
using Automate.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Automate.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly MongoDBService _mongoService;
        private readonly INavigationService _navigationService;

        private Window _window;

		private readonly Dictionary<string, List<string>> _errors = new();

		public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ICommand AuthenticateCommand { get; }
        public ICommand PasswordChangedCommand { get; }
        public bool HasErrors => _errors.Count > 0;
        public bool HasPasswordErrors => _errors.ContainsKey(nameof(Password)) && _errors[nameof(Password)].Any();

        public LoginViewModel(Window openedWindow, INavigationService navigationService = null)
        {
            _mongoService = new MongoDBService("AutomateDB");
            AuthenticateCommand = new RelayCommand(Authenticate);
            _navigationService = navigationService ?? new NavigationService();
            _window = openedWindow;
        }

        public Window Window
        {
            get => _window;
        }

		private string? _username;
		public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ValidateProperty(nameof(Username));
            }
        }

		private string? _password;
		public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ValidateProperty(nameof(Password));
            }
        }

        public string ErrorMessages
        {
            get
            {
                var allErrors = new List<string>();
                foreach (var errorList in _errors.Values)
                {
                    allErrors.AddRange(errorList);
                }
                allErrors.RemoveAll(error => string.IsNullOrWhiteSpace(error));

                return string.Join("\n", allErrors);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Authenticate()
        {
            ValidateProperty(nameof(Username));
            ValidateProperty(nameof(Password));

            if (!HasErrors)
            {
                var user = _mongoService.Authenticate(Username, Password);
                if (user is null)
                {
                    AddError("Username", "Nom d'utilisateur ou mot de passe invalide");
                    AddError("Password", string.Empty);
                }
                else
                {
                    _navigationService.NavigateTo<HomeWindow>(null, user.IsAdmin);
                    if(Window is not null)
                        _navigationService.CloseWindow("LoginWindowLaVue");
                }

            }
        }

        private void ValidateProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Username):
                    if (string.IsNullOrEmpty(Username))
                    {
                        AddError(nameof(Username), "Le nom d'utilisateur ne peut pas être vide.");
                    }
                    else
                    {
                        RemoveError(nameof(Username));
                    }
                    break;

                case nameof(Password):
                    if (string.IsNullOrEmpty(Password))
                    {
                        AddError(nameof(Password), "Le mot de passe ne peut pas être vide.");
                    }
                    else
                    {
                        RemoveError(nameof(Password));
                    }
                    break;
            }
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
               ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
               ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName)); 
            }

            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return _errors[propertyName];
        }
    }
}
