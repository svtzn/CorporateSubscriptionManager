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

        public EmployeeViewModel()
        {
            LoadData();
            NewRequestCommand = new RelayCommand(NewRequestExecute);
        }

        private void LoadData()
        {
            Assignments.Clear();
            var userAssignments = MockData.GetAssignmentsForEmployee(CurrentUser.Current.EmployeeID);
            foreach (var assign in userAssignments)
            {
                Assignments.Add(assign);
            }
        }

        private void NewRequestExecute(object parameter)
        {
            var form = new RequestFormWindow();
            form.ShowDialog();
            // После закрытия - обновить данные, если нужно (для просмотра заявок, но по ТЗ Employee видит только подписки)
            LoadData(); // На случай, если заявка сразу одобрена, но пока нет
        }
    }
}