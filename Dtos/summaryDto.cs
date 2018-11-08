using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SummaryDto
    {
        public int ShopId { get; set; }
        public int Number_Docket { get; set; }
        public decimal Total_Discount { get; set; }
        //public IEnumerable<PaymentViewModel> Payment { get; set; }
        public decimal Refund_Amount { get; set; }
        public int Refund_Number_Of_Item { get; set; }
        public decimal Total_Amount { get; set; }
    }
}
