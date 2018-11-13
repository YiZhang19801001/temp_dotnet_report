using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SalesByItemDto
    {
        public string ItemName { get; set; }
        public SalesItem DataSet { get; set; }
        public SalesItem DataSet_Compared { get; set; }
    }

    public class SalesItem {

        public double Quantity { get; set; }
        public decimal Total_amount { get; set; }
    }
}
