using System;
using System.Collections.Generic;
using CorporateSubscriptionManager.Models;

namespace CorporateSubscriptionManager.Services
{
    public static class MockData
    {
        private static readonly SqlDataService _db;

        static MockData()
        {
            _db = new SqlDataService(ConnectionProvider.DefaultConnection);
        }

        // Employees
        public static List<Employee> GetEmployees() => _db.GetEmployees();
        public static Employee GetEmployee(int id) => _db.GetEmployee(id);
        public static int AddEmployee(Employee e) => _db.AddEmployee(e);

        // New: поиск по логину/паролю
        public static Employee GetEmployeeByLogin(string login, string password) => _db.GetEmployeeByLogin(login, password);

        // Departments
        public static List<Department> GetDepartments() => _db.GetDepartments();
        public static Department GetDepartment(int id) => _db.GetDepartment(id);

        // Services
        public static List<Service> GetServices() => _db.GetServices();
        public static Service GetService(int id) => _db.GetService(id);
        public static int AddService(Service s) => _db.AddService(s);
        public static void UpdateService(Service s) => _db.UpdateService(s);

        // Requests
        public static List<SubscriptionRequest> GetAllRequests() => _db.GetAllRequests();
        public static List<SubscriptionRequest> GetRequests() => _db.GetRequests(); // для ManagerViewModel
        public static List<SubscriptionRequest> GetRequestsForEmployee(int employeeId) => _db.GetRequestsForEmployee(employeeId);
        public static int AddRequest(SubscriptionRequest r) => _db.AddRequest(r);
        public static void UpdateRequest(SubscriptionRequest r) => _db.UpdateRequest(r);

        // Assignments
        public static List<SubscriptionAssignment> GetAllAssignments() => _db.GetAllAssignments();
        public static List<SubscriptionAssignment> GetAssignmentsForEmployee(int employeeId) => _db.GetAssignmentsForEmployee(employeeId);
        public static int AddAssignment(SubscriptionAssignment a) => _db.AddAssignment(a);
        public static void UpdateAssignment(SubscriptionAssignment a) => _db.UpdateAssignment(a);
    }
}