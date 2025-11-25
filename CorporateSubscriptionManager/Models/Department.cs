using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateSubscriptionManager.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [ForeignKey("Manager")]
        public int? ManagerID { get; set; } // Nullable, как в ТЗ (ManagerID из Employees)

        public string Name { get; set; }

        public virtual Employee Manager { get; set; }
    }
}
