using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SummaryDetailDto
    {
        public double Total_Sales { get; set; }
        public double Total_Refund { get; set; }
        public int Number_Of_Transactions { get; set; }
        public double Total_Discount { get; set; }
        public double Avg_Sales_Value { get; set; }
        public double Avg_Item_Per_Sale { get; set; }

        public double[] Hourly_Sales { get; set; }

        public List<PaymentDetail> PaymentSum { get; set; }
        public List<CustomDataItem> CustomDataSum { get; set; }
    }
}
