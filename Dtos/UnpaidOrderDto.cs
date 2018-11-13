using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class UnpaidOrderDto
    {
        public string customer { get; set; }
        public decimal total_amount { get; set; }
        public decimal total_amount_compared { get; set; }
    }
}