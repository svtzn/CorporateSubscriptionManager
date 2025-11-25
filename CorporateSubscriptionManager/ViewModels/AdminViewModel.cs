using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CorporateSubscriptionManager.Models;
using CorporateSubscriptionManager.Services;

namespace CorporateSubscriptionManager.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        public ObservableCollection<Service> Services { get; } = new ObservableCollection<Service>();
        private Service _selectedService;
        public Service SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(); }
        }

        public RelayCommand NewServiceCommand { get; }
        public RelayCommand SaveServiceCommand { get; }
        public RelayCommand ToggleServiceActiveCommand { get; }

        public ObservableCollection<SubscriptionRequest> Requests { get; } = new ObservableCollection<SubscriptionRequest>();
        public RelayCommand ApproveRequestCommand { get; }
        public RelayCommand RejectRequestCommand { get; }

        public ObservableCollection<SubscriptionAssignment> Assignments { get; } = new ObservableCollection<SubscriptionAssignment>();
        public RelayCommand ExtendAssignmentCommand { get; }
        public RelayCommand SuspendAssignmentCommand { get; }
        public RelayCommand DeactivateAssignmentCommand { get; }

        // Простые данные для отчёта
        private string _reportText;
        public string ReportText { get => _reportText; set { _reportText = value; OnPropertyChanged(); } }

        public AdminViewModel()
        {
            NewServiceCommand = new RelayCommand(_ => NewService());
            SaveServiceCommand = new RelayCommand(_ => SaveService(), _ => SelectedService != null && !string.IsNullOrWhiteSpace(SelectedService.Name));
            ToggleServiceActiveCommand = new RelayCommand(p => ToggleServiceActive(p as Service));

            ApproveRequestCommand = new RelayCommand(p => ApproveRequest(p as SubscriptionRequest));
            RejectRequestCommand = new RelayCommand(p => RejectRequest(p as SubscriptionRequest));

            ExtendAssignmentCommand = new RelayCommand(p => ChangeAssignmentPeriod(p as SubscriptionAssignment, extend: true));
            SuspendAssignmentCommand = new RelayCommand(p => ChangeAssignmentStatus(p as SubscriptionAssignment, "Suspended"));
            DeactivateAssignmentCommand = new RelayCommand(p => ChangeAssignmentStatus(p as SubscriptionAssignment, "Expired"));

            LoadAll();
        }

        public void LoadAll()
        {
            LoadServices();
            LoadRequests();
            LoadAssignments();
            GenerateSimpleReport();
        }

        private void LoadServices()
        {
            Services.Clear();
            foreach (var s in MockData.GetServices().OrderBy(s => s.Name))
            {
                Services.Add(s);
            }
        }

        private void NewService()
        {
            SelectedService = new Service { IsActive = true };
        }

        private void SaveService()
        {
            if (SelectedService.ServiceID == 0)
            {
                // New
                MockData.AddService(SelectedService);
            }
            else
            {
                MockData.UpdateService(SelectedService);
            }
            LoadServices();
            SelectedService = null;
        }

        private void ToggleServiceActive(Service svc)
        {
            if (svc == null) return;
            svc.IsActive = !svc.IsActive;
            MockData.UpdateService(svc);
            LoadServices();
        }

        private void LoadRequests()
        {
            Requests.Clear();
            // Показываем только незавершённые заявки (не Approved и не Rejected)
            var all = MockData.GetAllRequests()
                .Where(r => !string.Equals(r.Status, "Approved", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(r.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.RequestDate)
                .ToList();
            foreach (var r in all)
            {
                r.Service = r.Service ?? MockData.GetService(r.ServiceID);
                r.Employee = r.Employee ?? MockData.GetEmployee(r.EmployeeID);
                r.Manager = r.Manager ?? (r.ManagerID.HasValue ? MockData.GetEmployee(r.ManagerID.Value) : null);
                r.Approver = r.Approver ?? (r.ApproverID.HasValue ? MockData.GetEmployee(r.ApproverID.Value) : null);
                // default start date on card
                if (!r.SelectedStartDate.HasValue) r.SelectedStartDate = DateTime.Now.Date;
                Requests.Add(r);
            }
        }

        private void ApproveRequest(SubscriptionRequest req)
        {
            if (req == null) return;

            var startDate = req.SelectedStartDate ?? DateTime.Now.Date;
            var svc = MockData.GetService(req.ServiceID);
            var endDate = ComputeEndDate(svc, startDate);

            req.Status = "Approved";
            req.DecisionDate = DateTime.Now;
            req.ApproverID = CurrentUser.Current.EmployeeID; // админ
            MockData.UpdateRequest(req);

            var assign = new SubscriptionAssignment
            {
                EmployeeID = req.EmployeeID,
                ServiceID = req.ServiceID,
                RequestID = req.RequestID,
                ApproverID = req.ApproverID,
                Status = "Active",
                StartDate = startDate,
                EndDate = endDate
            };
            MockData.AddAssignment(assign);

            MessageBox.Show($"Заявка #{req.RequestID} одобрена.\nПериод: {startDate:dd.MM.yyyy} — {endDate:dd.MM.yyyy}", "Одобрено");
            LoadRequests();
            LoadAssignments();
            GenerateSimpleReport();
        }

        private void RejectRequest(SubscriptionRequest req)
        {
            if (req == null) return;
            req.Status = "Rejected";
            req.DecisionDate = DateTime.Now;
            req.ApproverID = CurrentUser.Current.EmployeeID;
            MockData.UpdateRequest(req);
            MessageBox.Show($"Заявка #{req.RequestID} отклонена.");
            LoadRequests();
            GenerateSimpleReport();
        }

        private void LoadAssignments()
        {
            Assignments.Clear();
            foreach (var a in MockData.GetAllAssignments().OrderByDescending(a => a.StartDate))
            {
                a.Service = a.Service ?? MockData.GetService(a.ServiceID);
                a.Employee = a.Employee ?? MockData.GetEmployee(a.EmployeeID);
                a.Approver = a.Approver ?? (a.ApproverID.HasValue ? MockData.GetEmployee(a.ApproverID.Value) : null);
                Assignments.Add(a);
            }
        }

        private void ChangeAssignmentPeriod(SubscriptionAssignment assign, bool extend = false)
        {
            if (assign == null) return;
            if (extend)
            {
                // продлеваем на интервал сервиса
                var svc = MockData.GetService(assign.ServiceID);
                var newEnd = ComputeEndDate(svc, assign.EndDate ?? assign.StartDate);
                assign.EndDate = newEnd;
                MockData.UpdateAssignment(assign);
                MessageBox.Show($"Подписка #{assign.AssignmentID} продлена до {newEnd:dd.MM.yyyy}");
            }
            LoadAssignments();
            GenerateSimpleReport();
        }

        private void ChangeAssignmentStatus(SubscriptionAssignment assign, string newStatus)
        {
            if (assign == null) return;
            assign.Status = newStatus;
            if (newStatus == "Expired") assign.EndDate = DateTime.Now.Date;
            MockData.UpdateAssignment(assign);
            LoadAssignments();
            GenerateSimpleReport();
        }

        private DateTime ComputeEndDate(Service svc, DateTime start)
        {
            if (svc == null || string.IsNullOrWhiteSpace(svc.RenewalCycle))
                return start.AddMonths(1);

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

        private void GenerateSimpleReport()
        {
            // Простой отчёт: общие затраты (текущие активные подписки * цена)
            var active = MockData.GetAllAssignments().Where(a => a.Status == "Active").ToList();
            decimal total = 0;
            foreach (var a in active)
            {
                var svc = MockData.GetService(a.ServiceID);
                if (svc != null) total += svc.PricePerUser;
            }

            var byService = active.GroupBy(a => a.ServiceID)
                .Select(g =>
                {
                    var svc = MockData.GetService(g.Key);
                    return new { Service = svc?.Name ?? g.Key.ToString(), Count = g.Count(), Cost = (svc?.PricePerUser ?? 0) * g.Count() };
                }).OrderByDescending(x => x.Cost).ToList();

            ReportText = $"Активных подписок: {active.Count}, Общие ежемесячные расходы (ориентировочно): {total:C}\n";
            ReportText += "По сервисам:\n";
            foreach (var s in byService)
            {
                ReportText += $"{s.Service}: пользователей {s.Count}, стоимость {s.Cost:C}\n";
            }

            // Новый блок: отчёт по отделам
            var departments = MockData.GetDepartments();
            ReportText += "\nПо отделам:\n";
            var byDept = active.GroupBy(a =>
            {
                var emp = MockData.GetEmployee(a.EmployeeID);
                return emp?.DepartmentID ?? 0;
            })
            .Select(g =>
            {
                var deptId = g.Key;
                var dept = departments.FirstOrDefault(d => d.DepartmentID == deptId);
                var deptName = dept?.Name ?? $"Dept#{deptId}";
                decimal deptCost = 0;
                foreach (var a in g)
                {
                    var svc = MockData.GetService(a.ServiceID);
                    if (svc != null) deptCost += svc.PricePerUser;
                }
                return new { DeptName = deptName, Count = g.Count(), Cost = deptCost };
            })
            .OrderByDescending(x => x.Cost)
            .ToList();

            foreach (var d in byDept)
            {
                ReportText += $"{d.DeptName}: пользователей {d.Count}, стоимость {d.Cost:C}\n";
            }

            OnPropertyChanged(nameof(ReportText));
        }
    }
}
