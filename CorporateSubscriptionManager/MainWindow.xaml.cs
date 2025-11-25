using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.ViewModels;

namespace CorporateSubscriptionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadRoleBasedUI();
        }

        private void LoadRoleBasedUI()
        {
            var user = CurrentUser.Current;
            if (user == null) { MessageBox.Show("No user"); return; }
            if (user == null) { Close(); return; }

            Title = $"Добро пожаловать, {user.Name} ({user.Role})";

            // Placeholder: в следующих этапах заменим на реальные Views/UserControls
            switch (user.Role)
            {
                case "Employee":
                    var employeeView = new Views.EmployeeView();
                    employeeView.DataContext = new EmployeeViewModel();
                    // Вставь в XAML MainWindow <ContentControl x:Name="MainContent"/> и здесь:
                    MainContent.Content = employeeView;
                    break;
                //case "Manager":
                //    DataContext = new ManagerViewModel();
                //    // + функционал одобрения заявок своего отдела
                //    break;
                //case "Admin":
                //    DataContext = new AdminViewModel();
                //    // DataGrid для сервисов, заявок и т.д.
                //    break;
            }
        }
    }
}