using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SummaryDtoWithCompare
    {
        public int ShopId { get; set; }
        public decimal Total_Sales { get; set; }
        public decimal Total_Refund { get; set; }
        public int Number_Of_Transactions { get; set; }
        public decimal Total_Discount { get; set; }
        public decimal Avg_Sales_Value { get; set; }
        public double Avg_Item_Per_Sale { get; set; }
        public decimal Total_Sales_Compare { get; set; }
        public decimal Total_Refund_Compare { get; set; }
        public int Number_Of_Transactions_Compare { get; set; }
        public decimal Total_Discount_Compare { get; set; }
        public decimal Avg_Sales_Value_Compare { get; set; }
        public double Avg_Item_Per_Sale_Compare_Compare { get; set; }
    }
}
