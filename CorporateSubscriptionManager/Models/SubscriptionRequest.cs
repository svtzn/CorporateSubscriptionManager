using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateSubscriptionManager.Models
{
    public class SubscriptionRequest
    {
        [Key]
        public int RequestID { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }

        [ForeignKey("Manager")]
        public int? ManagerID { get; set; } // Nullable

        public DateTime RequestDate { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string Comment { get; set; }
        public DateTime? DecisionDate { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Service Service { get; set; }
        public virtual Employee Manager { get; set; }
    }
}
