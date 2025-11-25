using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateSubscriptionManager.Models
{
    public class Service
    {
        [Key]
        public int ServiceID { get; set; }

        public decimal PricePerUser { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // "SaaS" etc.
        public string RenewalCycle { get; set; } // e.g. "Monthly"
        public bool IsActive { get; set; }
    }
}
