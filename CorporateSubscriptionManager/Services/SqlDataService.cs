using CorporateSubscriptionManager.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CorporateSubscriptionManager.Services
{
    public class SqlDataService
    {
        private readonly string _connectionString;

        public SqlDataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        // Employees
        public List<Employee> GetEmployees()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT EmployeeID, DepartmentID, Name, Role, IsActive, Login, Password FROM Employees";
                return conn.Query<Employee>(sql).AsList();
            }
        }

        public Employee GetEmployee(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT EmployeeID, DepartmentID, Name, Role, IsActive, Login, Password FROM Employees WHERE EmployeeID = @Id";
                return conn.QuerySingleOrDefault<Employee>(sql, new { Id = id });
            }
        }

        public Employee GetEmployeeByLogin(string login, string password)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
SELECT EmployeeID, DepartmentID, Name, Role, IsActive, Login, Password
FROM Employees
WHERE Login = @Login AND Password = @Password AND IsActive = 1";
                return conn.QuerySingleOrDefault<Employee>(sql, new { Login = login, Password = password });
            }
        }

        public int AddEmployee(Employee emp)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
INSERT INTO Employees (DepartmentID, Name, Role, IsActive, Login, Password)
VALUES (@DepartmentID, @Name, @Role, @IsActive, @Login, @Password);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                return conn.QuerySingle<int>(sql, emp);
            }
        }

        // Departments
        public List<Department> GetDepartments()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT DepartmentID, Name, ManagerID FROM Departments";
                return conn.Query<Department>(sql).AsList();
            }
        }

        public Department GetDepartment(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT DepartmentID, Name, ManagerID FROM Departments WHERE DepartmentID = @Id";
                return conn.QuerySingleOrDefault<Department>(sql, new { Id = id });
            }
        }

        // Services
        public List<Service> GetServices()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT ServiceID, Name, Type, PricePerUser, RenewalCycle, IsActive FROM Services";
                return conn.Query<Service>(sql).AsList();
            }
        }

        public Service GetService(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = "SELECT ServiceID, Name, Type, PricePerUser, RenewalCycle, IsActive FROM Services WHERE ServiceID = @Id";
                return conn.QuerySingleOrDefault<Service>(sql, new { Id = id });
            }
        }

        public int AddService(Service svc)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
INSERT INTO Services (Name, Type, PricePerUser, RenewalCycle, IsActive)
VALUES (@Name, @Type, @PricePerUser, @RenewalCycle, @IsActive);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                return conn.QuerySingle<int>(sql, svc);
            }
        }

        public void UpdateService(Service svc)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
UPDATE Services
SET Name = @Name, Type = @Type, PricePerUser = @PricePerUser, RenewalCycle = @RenewalCycle, IsActive = @IsActive
WHERE ServiceID = @ServiceID";
                conn.Execute(sql, svc);
            }
        }

        // SubscriptionRequests
        public List<SubscriptionRequest> GetAllRequests()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
SELECT RequestID, EmployeeID, ServiceID, ManagerID, ApproverID, RequestDate, Status, Comment, DecisionDate
FROM SubscriptionRequests";
                return conn.Query<SubscriptionRequest>(sql).AsList();
            }
        }

        public List<SubscriptionRequest> GetRequests()
        {
            return GetAllRequests();
        }

        public List<SubscriptionRequest> GetRequestsForEmployee(int employeeId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
SELECT RequestID, EmployeeID, ServiceID, ManagerID, ApproverID, RequestDate, Status, Comment, DecisionDate
FROM SubscriptionRequests
WHERE EmployeeID = @EmployeeID";
                return conn.Query<SubscriptionRequest>(sql, new { EmployeeID = employeeId }).AsList();
            }
        }

        public int AddRequest(SubscriptionRequest req)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                req.RequestDate = req.RequestDate == default ? DateTime.Now : req.RequestDate;
                req.Status = string.IsNullOrWhiteSpace(req.Status) ? "Pending" : req.Status;

                const string sql = @"
INSERT INTO SubscriptionRequests (EmployeeID, ServiceID, ManagerID, ApproverID, RequestDate, Status, Comment, DecisionDate)
VALUES (@EmployeeID, @ServiceID, @ManagerID, @ApproverID, @RequestDate, @Status, @Comment, @DecisionDate);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                return conn.QuerySingle<int>(sql, req);
            }
        }

        public void UpdateRequest(SubscriptionRequest req)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
UPDATE SubscriptionRequests
SET EmployeeID = @EmployeeID, ServiceID = @ServiceID, ManagerID = @ManagerID, ApproverID = @ApproverID,
    RequestDate = @RequestDate, Status = @Status, Comment = @Comment, DecisionDate = @DecisionDate
WHERE RequestID = @RequestID";
                conn.Execute(sql, req);
            }
        }

        // SubscriptionAssignments
        public List<SubscriptionAssignment> GetAllAssignments()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
SELECT AssignmentID, EmployeeID, ServiceID, RequestID, ApproverID, Status, StartDate, EndDate
FROM SubscriptionAssignments";
                return conn.Query<SubscriptionAssignment>(sql).AsList();
            }
        }

        public List<SubscriptionAssignment> GetAssignmentsForEmployee(int employeeId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
SELECT AssignmentID, EmployeeID, ServiceID, RequestID, ApproverID, Status, StartDate, EndDate
FROM SubscriptionAssignments
WHERE EmployeeID = @EmployeeID";
                return conn.Query<SubscriptionAssignment>(sql, new { EmployeeID = employeeId }).AsList();
            }
        }

        public int AddAssignment(SubscriptionAssignment assign)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
INSERT INTO SubscriptionAssignments (EmployeeID, ServiceID, RequestID, ApproverID, Status, StartDate, EndDate)
VALUES (@EmployeeID, @ServiceID, @RequestID, @ApproverID, @Status, @StartDate, @EndDate);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                return conn.QuerySingle<int>(sql, assign);
            }
        }

        public void UpdateAssignment(SubscriptionAssignment assign)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                const string sql = @"
UPDATE SubscriptionAssignments
SET EmployeeID = @EmployeeID, ServiceID = @ServiceID, RequestID = @RequestID, ApproverID = @ApproverID,
    Status = @Status, StartDate = @StartDate, EndDate = @EndDate
WHERE AssignmentID = @AssignmentID";
                conn.Execute(sql, assign);
            }
        }
    }
}
