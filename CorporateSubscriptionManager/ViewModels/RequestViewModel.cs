using System.Collections.ObjectModel;
using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class RequestViewModel : BaseViewModel
    {
        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();

        private Service _selectedService;
        public Service SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(); }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public RelayCommand SubmitCommand { get; }
        public RelayCommand CancelCommand { get; }

        private readonly Window _window;

        public RequestViewModel(Window window)
        {
            _window = window;
            LoadServices();
            SubmitCommand = new RelayCommand(SubmitExecute, CanSubmit);
            CancelCommand = new RelayCommand(CancelExecute);
        }

        private void LoadServices()
        {
            Services.Clear();
            foreach (var svc in MockData.GetServices())
            {
                Services.Add(svc);
            }
        }

        private bool CanSubmit(object parameter) => SelectedService != null && !string.IsNullOrWhiteSpace(Comment);

        private void SubmitExecute(object parameter)
        {
            var req = new SubscriptionRequest
            {
                EmployeeID = CurrentUser.Current.EmployeeID,
                ServiceID = SelectedService.ServiceID,
                Comment = Comment
            };
            MockData.AddRequest(req);
            MessageBox.Show("Заявка подана!");
            _window.Close();
        }

        private void CancelExecute(object parameter)
        {
            _window.Close();
        }
    }
}