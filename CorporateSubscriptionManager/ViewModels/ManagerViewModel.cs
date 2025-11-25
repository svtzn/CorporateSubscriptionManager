using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class ManagerViewModel : BaseViewModel
    {
        public ObservableCollection<SubscriptionRequest> Requests { get; set; } = new ObservableCollection<SubscriptionRequest>();

        public RelayCommand ApproveCommand { get; }
        public RelayCommand RejectCommand { get; }

        public ManagerViewModel()
        {
            LoadRequests();
            ApproveCommand = new RelayCommand(ApproveExecute);
            RejectCommand = new RelayCommand(RejectExecute);
        }

        public void LoadRequests()
        {
            Requests.Clear();
            // Показываем менеджеру заявки только его отдела, только незавершённые (не Approved и не Rejected)
            var reqs = MockData.GetRequests()
                .Where(r => r.ManagerID == CurrentUser.Current.EmployeeID
                            && !string.Equals(r.Status, "Approved", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(r.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            foreach (var r in reqs)
            {
                r.Service = r.Service ?? MockData.GetService(r.ServiceID);
                r.Employee = r.Employee ?? MockData.GetEmployee(r.EmployeeID);
                r.Approver = r.Approver ?? (r.ApproverID.HasValue ? MockData.GetEmployee(r.ApproverID.Value) : null);
                // По умолчанию предложим дату начала — сегодня
                if (!r.SelectedStartDate.HasValue) r.SelectedStartDate = DateTime.Now.Date;
                Requests.Add(r);
            }
        }

        private void ApproveExecute(object parameter)
        {
            if (!(parameter is SubscriptionRequest req)) return;

            // Используем выбранную на карточке дату или сегодня
            var startDate = req.SelectedStartDate ?? DateTime.Now.Date;

            // Вычисляем дату окончания по RenewalCycle сервиса
            var svc = MockData.GetService(req.ServiceID);
            var endDate = ComputeEndDate(svc, startDate);

            // Обновляем запрос: статус, дата решения, кто одобрил
            req.Status = "Approved";
            req.DecisionDate = DateTime.Now;
            req.ApproverID = CurrentUser.Current.EmployeeID;
            MockData.UpdateRequest(req);

            // Создаём назначение подписки с ApproverID
            var assign = new SubscriptionAssignment
            {
                EmployeeID = req.EmployeeID,
                ServiceID = req.ServiceID,
                RequestID = req.RequestID,
                ApproverID = CurrentUser.Current.EmployeeID,
                Status = "Active",
                StartDate = startDate,
                EndDate = endDate
            };
            MockData.AddAssignment(assign);

            MessageBox.Show($"Заявка #{req.RequestID} одобрена.\nApproverID: {req.ApproverID}\nПериод: {startDate:dd.MM.yyyy} — {endDate:dd.MM.yyyy}", "Одобрено");

            LoadRequests();
        }

        private void RejectExecute(object parameter)
        {
            if (!(parameter is SubscriptionRequest req)) return;

            req.Status = "Rejected";
            req.DecisionDate = DateTime.Now;
            req.ApproverID = CurrentUser.Current.EmployeeID;
            MockData.UpdateRequest(req);

            MessageBox.Show($"Заявка #{req.RequestID} отклонена.");
            LoadRequests();
        }

        private DateTime ComputeEndDate(Service svc, DateTime start)
        {
            if (svc == null || string.IsNullOrWhiteSpace(svc.RenewalCycle))
                return start.AddMonths(1); // fallback

            var rc = svc.RenewalCycle.Trim().ToLowerInvariant();

            if (rc.Contains("month")) return start.AddMonths(1);
            if (rc.Contains("quarter")) return start.AddMonths(3);
            if (rc.Contains("year") || rc.Contains("annual")) return start.AddYears(1);
            if (rc.Contains("day"))
            {
                var num = ExtractLeadingNumber(rc);
                if (num > 0) return start.AddDays(num);
            }
            var months = ExtractLeadingNumber(rc);
            if (months > 0) return start.AddMonths((int)months);

            return start.AddMonths(1);
        }

        private int ExtractLeadingNumber(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            var numStr = new string(s.TakeWhile(ch => char.IsDigit(ch)).ToArray());
            if (int.TryParse(numStr, out var n)) return n;
            return 0;
        }
    }
}
