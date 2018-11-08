using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SalesByDayDto
    {
        public DateTime Single_date { get; set; }
        public decimal Sum_amount { get; set; }
    }
}
