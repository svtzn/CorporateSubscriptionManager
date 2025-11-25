using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateSubscriptionManager.Models
{
    public class SubscriptionAssignment
    {
        [Key]
        public int AssignmentID { get; set; } // В ТЗ AssigmentID

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }

        [ForeignKey("Request")]
        public int? RequestID { get; set; } // Nullable

        [ForeignKey("Approver")]
        public int? ApproverID { get; set; } // Manager или Admin

        public string Status { get; set; } // "Active", "Expired", "Suspended"
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Service Service { get; set; }
        public virtual SubscriptionRequest Request { get; set; }
        public virtual Employee Approver { get; set; }

        public string ServiceName
        {
            get
            {
                if (Service != null) return Service.Name;
                var svc = CorporateSubscriptionManager.Services.MockData.GetService(ServiceID);
                return svc?.Name ?? string.Empty;
            }
        }
    }
}
