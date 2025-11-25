using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorporateSubscriptionManager.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }

        public string Name { get; set; }
        public string Role { get; set; } // "Employee", "Manager", "Admin"
        public bool IsActive { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } // Для mock; в реале хэшировать

        public virtual Department Department { get; set; }
    }
}
