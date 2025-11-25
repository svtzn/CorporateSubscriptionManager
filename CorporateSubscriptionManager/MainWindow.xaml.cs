using System.Windows;
using System.Windows.Controls;
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
            if (user == null) { Close(); return; }

            Title = $"Добро пожаловать, {user.Name} ({user.Role})";

            NavPanel.Children.Clear();

            var btnMySubscriptions = new Button { Content = "Мои подписки", Margin = new Thickness(0, 0, 0, 8) };
            btnMySubscriptions.Click += (s, e) => ShowEmployeeView();
            NavPanel.Children.Add(btnMySubscriptions);

            var btnNewRequest = new Button { Content = "Подать заявку", Margin = new Thickness(0, 0, 0, 8) };
            btnNewRequest.Click += (s, e) => ShowRequestView();
            NavPanel.Children.Add(btnNewRequest);

            if (user.Role == "Manager")
            {
                var btnManageRequests = new Button { Content = "Управление заявками", Margin = new Thickness(0, 0, 0, 8) };
                btnManageRequests.Click += (s, e) => ShowManagerView();
                NavPanel.Children.Add(btnManageRequests);
            }

            if (user.Role == "Admin")
            {
                var btnAdmin = new Button { Content = "Администрирование", Margin = new Thickness(0, 0, 0, 8) };
                btnAdmin.Click += (s, e) => ShowAdminView();
                NavPanel.Children.Add(btnAdmin);
            }

            ShowEmployeeView();
        }

        private void ShowEmployeeView()
        {
            var employeeView = new Views.EmployeeView();
            employeeView.DataContext = new EmployeeViewModel();
            MainContent.Content = employeeView;
        }

        private void ShowRequestView()
        {
            var requestView = new Views.RequestView(onSubmitted: () =>
            {
                if (MainContent.Content is Views.EmployeeView ev && ev.DataContext is EmployeeViewModel vm)
                {
                    vm.GetType().GetMethod("LoadData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(vm, null);
                }
                ShowEmployeeView();
            },
            onCancelled: () => ShowEmployeeView());

            MainContent.Content = requestView;
        }

        private void ShowManagerView()
        {
            var managerView = new Views.ManagerView();
            MainContent.Content = managerView;
        }

        private void ShowAdminView()
        {
            var adminView = new Views.AdminView();
            MainContent.Content = adminView;
        }
    }
}