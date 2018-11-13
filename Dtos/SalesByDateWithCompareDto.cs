using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SalesByDateWithCompareDto
    {
        public int Id { get; set; }
        public SalesByDayDto Value { get; set; }
        public SalesByDayDto Value_Compared { get; set; }
    }
}
