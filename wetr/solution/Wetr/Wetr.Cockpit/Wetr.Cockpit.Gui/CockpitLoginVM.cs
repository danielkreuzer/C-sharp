using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Cockpit.Gui {
    public class CockpitLoginVM : INotifyPropertyChanged {
        private readonly IUserManager _userManager;
        private string _enteredUsername;
        private string _enteredPassword;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Window _currentWindow;
        private string _errorText;
        private SynchronizationContext _context;

        public CockpitLoginVM(IUserManager userManager, Window currentWindow) {
            this._context = SynchronizationContext.Current;
            this._userManager = userManager;
            this._currentWindow = currentWindow;
            this._enteredUsername = "";
            this._enteredPassword = "";
            ErrorTextLogin = "";
        }

        private async void OpenWindow(string password) {
            ErrorTextLogin = "Processing...";
            if (await _userManager.CheckLogin(_enteredUsername, password)) {
                _context.Send(async x => {
                    User user = await _userManager.GetUserByUsername(_enteredUsername);
                    _context.Send(y => {
                        ErrorTextLogin = "Wait while window opens!";
                        CockpitWindow window = new CockpitWindow(user);
                        window.Show();
                        _currentWindow.Close();
                    }, null);
                }, null);
                
            }
            else {
                _context.Send(x => {
                    ErrorTextLogin = "Invalid username or password!";
                }, null);
            }
        }
     

        public string ErrorTextLogin {
            get => _errorText;
            set {
                _errorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorTextLogin)));
            }
        }

        private void CancelLogin() {
            CockpitWindow window = new CockpitWindow();
            window.Show();
            _currentWindow.Close();
        }

        public string Username {
            get => _enteredUsername;
            set {
                _enteredUsername = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }

        public string Password {
            get => _enteredPassword;
            set {
                _enteredPassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand {
            get { return _loginCommand ?? (_loginCommand = new RelayCommand(param => Task.Run(() => OpenWindow(((PasswordBox) param).Password)))); }
        }

        private ICommand _cancelCommand;

        public ICommand CancelCommand {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => CancelLogin())); }
        }
    }
}
