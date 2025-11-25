using System.Collections.ObjectModel;
using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        public ObservableCollection<SubscriptionAssignment> Assignments { get; set; } = new ObservableCollection<SubscriptionAssignment>();

        public RelayCommand NewRequestCommand { get; }

        private readonly SqlDataService _dataService;

        public EmployeeViewModel()
        {
            _dataService = new SqlDataService(ConnectionProvider.DefaultConnection);
            LoadData();
            NewRequestCommand = new RelayCommand(NewRequestExecute);
        }

        private void LoadData()
        {
            Assignments.Clear();
            try
            {
                var userAssignments = _dataService.GetAssignmentsForEmployee(CurrentUser.Current.EmployeeID);
                foreach (var assign in userAssignments)
                {
                    Assignments.Add(assign);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке назначений: " + ex.Message);
            }
        }

        private void NewRequestExecute(object parameter)
        {
            var form = new RequestFormWindow();
            form.ShowDialog();
            LoadData();
        }
    }
}