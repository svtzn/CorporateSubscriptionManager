using System;
using System.Collections.Generic;
using System.Linq;
using CorporateSubscriptionManager.Models;

namespace CorporateSubscriptionManager.Services
{
    public static class MockData
    {
        private static List<Employee> _employees = new List<Employee>();
        private static List<Department> _departments = new List<Department>();
        private static List<Service> _services = new List<Service>();
        private static List<SubscriptionRequest> _requests = new List<SubscriptionRequest>();
        private static List<SubscriptionAssignment> _assignments = new List<SubscriptionAssignment>();

        static MockData()
        {
            // Sample Departments
            _departments.Add(new Department { DepartmentID = 1, Name = "IT", ManagerID = 1 });
            _departments.Add(new Department { DepartmentID = 2, Name = "Finance", ManagerID = 3 });
            _departments.Add(new Department { DepartmentID = 3, Name = "HR", ManagerID = 5 });

            // Sample Employees (логины/пароли для теста: admin/admin, manager/manager, employee/employee)
            _employees.Add(new Employee { EmployeeID = 1, DepartmentID = 1, Name = "Admin User", Role = "Admin", IsActive = true, Login = "admin", Password = "admin" });
            _employees.Add(new Employee { EmployeeID = 2, DepartmentID = 1, Name = "Employee IT1", Role = "Employee", IsActive = true, Login = "emp1", Password = "pass" });
            _employees.Add(new Employee { EmployeeID = 3, DepartmentID = 2, Name = "Manager Finance", Role = "Manager", IsActive = true, Login = "manager", Password = "manager" });
            _employees.Add(new Employee { EmployeeID = 4, DepartmentID = 2, Name = "Employee Finance1", Role = "Employee", IsActive = true, Login = "emp2", Password = "pass" });
            _employees.Add(new Employee { EmployeeID = 5, DepartmentID = 3, Name = "Manager HR", Role = "Manager", IsActive = true, Login = "managerhr", Password = "pass" });
            _employees.Add(new Employee { EmployeeID = 6, DepartmentID = 3, Name = "Employee HR1", Role = "Employee", IsActive = true, Login = "emp3", Password = "pass" });

            // Sample Services
            _services.Add(new Service { ServiceID = 1, Name = "Microsoft Office", Type = "SaaS", PricePerUser = 10.0m, RenewalCycle = "Monthly", IsActive = true });
            _services.Add(new Service { ServiceID = 2, Name = "Adobe Creative Cloud", Type = "SaaS", PricePerUser = 20.0m, RenewalCycle = "Annual", IsActive = true });
            _services.Add(new Service { ServiceID = 3, Name = "Slack", Type = "SaaS", PricePerUser = 5.0m, RenewalCycle = "Monthly", IsActive = true });
            _services.Add(new Service { ServiceID = 4, Name = "Zoom", Type = "SaaS", PricePerUser = 15.0m, RenewalCycle = "Monthly", IsActive = true });
            _services.Add(new Service { ServiceID = 5, Name = "GitHub", Type = "SaaS", PricePerUser = 4.0m, RenewalCycle = "Monthly", IsActive = true });

            // Sample Requests
            _requests.Add(new SubscriptionRequest { RequestID = 1, EmployeeID = 2, ServiceID = 1, ManagerID = 1, RequestDate = DateTime.Now.AddDays(-5), Status = "Pending", Comment = "Need for work" });
            _requests.Add(new SubscriptionRequest { RequestID = 2, EmployeeID = 4, ServiceID = 2, ManagerID = 3, RequestDate = DateTime.Now.AddDays(-3), Status = "Approved", DecisionDate = DateTime.Now.AddDays(-2) });

            // Sample Assignments
            _assignments.Add(new SubscriptionAssignment { AssignmentID = 1, EmployeeID = 2, ServiceID = 2, RequestID = 2, ApproverID = 3, Status = "Active", StartDate = DateTime.Now.AddDays(-2), EndDate = DateTime.Now.AddMonths(1) });


        }

        // Методы для Employees
        public static Employee GetEmployeeByLogin(string login, string password)
        {
            return _employees.FirstOrDefault(e => e.Login == login && e.Password == password && e.IsActive);
        }

        public static List<Employee> GetEmployees() => _employees;
        public static Employee GetEmployee(int id) => _employees.FirstOrDefault(e => e.EmployeeID == id);
        public static void AddEmployee(Employee emp) { emp.EmployeeID = _employees.Max(e => e.EmployeeID) + 1; _employees.Add(emp); }
        public static void UpdateEmployee(Employee emp) { var existing = GetEmployee(emp.EmployeeID); if (existing != null) { /* копируем свойства */ existing.Name = emp.Name; /* etc. */ } }
        public static void DeleteEmployee(int id) { _employees.RemoveAll(e => e.EmployeeID == id); }

        // для Services
        public static List<Service> GetServices() => _services;
        public static Service GetService(int id) => _services.FirstOrDefault(s => s.ServiceID == id);
        public static void AddService(Service svc) { svc.ServiceID = _services.Max(s => s.ServiceID) + 1; _services.Add(svc); }
        public static void UpdateService(Service svc) { var existing = GetService(svc.ServiceID); if (existing != null) { existing.Name = svc.Name; /* etc. */ } }
        public static void DeleteService(int id) { _services.RemoveAll(s => s.ServiceID == id); }

        // Для Requests
        public static List<SubscriptionRequest> GetRequests() => _requests;
        public static List<SubscriptionRequest> GetRequestsForManager(int managerId) => _requests.Where(r => r.ManagerID == managerId && r.Status == "Pending").ToList();
        // Добавить похожие для других
        // ...

        // Добавь в конец класса MockData

        // Для Requests
        public static List<SubscriptionRequest> GetRequestsForEmployee(int employeeId) => _requests.Where(r => r.EmployeeID == employeeId).ToList();
        public static void AddRequest(SubscriptionRequest req)
        {
            req.RequestID = _requests.Any() ? _requests.Max(r => r.RequestID) + 1 : 1;
            req.RequestDate = DateTime.Now;
            req.Status = "Pending";
            req.DecisionDate = null;
            // Авто-ManagerID: из отдела сотрудника
            var emp = GetEmployee(req.EmployeeID);
            if (emp != null)
            {
                var dept = _departments.FirstOrDefault(d => d.DepartmentID == emp.DepartmentID);
                if (dept != null) req.ManagerID = dept.ManagerID;
            }
            _requests.Add(req);
        }
        public static void UpdateRequest(SubscriptionRequest req)
        {
            var existing = _requests.FirstOrDefault(r => r.RequestID == req.RequestID);
            if (existing != null)
            {
                existing.Status = req.Status;
                existing.Comment = req.Comment;
                existing.DecisionDate = req.DecisionDate;
                // etc.
            }
        }

        // Для Assignments
        public static List<SubscriptionAssignment> GetAssignmentsForEmployee(int employeeId) => _assignments.Where(a => a.EmployeeID == employeeId).ToList();
        public static void AddAssignment(SubscriptionAssignment assign)
        {
            assign.AssignmentID = _assignments.Any() ? _assignments.Max(a => a.AssignmentID) + 1 : 1;
            _assignments.Add(assign);
        }
        // Похожие Update/Delete, если нужно
    }
}