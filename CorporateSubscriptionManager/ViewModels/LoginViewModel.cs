using System.Windows;
using System.Windows.Controls;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class LoginViewModel : BaseViewModel // Создай BaseViewModel с INotifyPropertyChanged ниже
    {
        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }

        private readonly Window _window;

        public LoginViewModel(Window window)
        {
            _window = window;
            LoginCommand = new RelayCommand(LoginExecute);
        }

        private void LoginExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            var user = MockData.GetEmployeeByLogin(Login, password);
            if (user != null)
            {
                CurrentUser.Current = user;
                var mainWindow = new MainWindow();
                mainWindow.Show();
                _window.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
    }
}