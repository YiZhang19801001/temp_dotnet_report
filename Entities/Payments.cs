using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
    public class Payments
    {
        [Key]
        public int payment_id { get; set; }
        public int docket_id { get; set; }
        public string paymenttype { get; set; }
        public decimal amount { get; set; }
        public DateTime docket_date { get; set; }
    }
}
