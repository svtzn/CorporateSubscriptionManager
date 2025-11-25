using CorporateSubscriptionManager.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CorporateSubscriptionManager
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            MessageBox.Show("LoginWindow created");
            DataContext = new ViewModels.LoginViewModel(this);
        }
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = DataContext as LoginViewModel;
                vm.LoginCommand.Execute(PasswordBox);
            }
        }

    }
}