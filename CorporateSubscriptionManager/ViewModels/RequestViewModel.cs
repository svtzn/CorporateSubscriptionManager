using System;
using System.Collections.ObjectModel;
using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class RequestViewModel : BaseViewModel
    {
        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();
        public ObservableCollection<SubscriptionRequest> Requests { get; set; } = new ObservableCollection<SubscriptionRequest>();

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

        // Внешние колбэки — вызываются после отправки/отмены (для вкладки)
        public Action OnSubmitted { get; set; }
        public Action OnCancelled { get; set; }

        // Сохраним совместимость: если используется Window, старый конструктор оставлен
        public RequestViewModel(Window window) : this()
        {
            // Закрывать окно при использовании старого паттерна
            OnSubmitted = () => window.Close();
            OnCancelled = () => window.Close();
        }

        // Вариант для использования в UserControl (вкладке)
        public RequestViewModel()
        {
            LoadServices();
            LoadRequests();
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

        public void LoadRequests()
        {
            Requests.Clear();
            var reqs = MockData.GetRequestsForEmployee(CurrentUser.Current.EmployeeID);
            foreach (var r in reqs)
            {
                // Заполним навигационные свойства на случай, если они не установлены
                r.Service = r.Service ?? MockData.GetService(r.ServiceID);
                r.Employee = r.Employee ?? MockData.GetEmployee(r.EmployeeID);
                Requests.Add(r);
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
            // Обновляем список карточек
            LoadRequests();
            // Сброс формы
            SelectedService = null;
            Comment = string.Empty;
            OnSubmitted?.Invoke();
        }

        private void CancelExecute(object parameter)
        {
            OnCancelled?.Invoke();
        }
    }
}