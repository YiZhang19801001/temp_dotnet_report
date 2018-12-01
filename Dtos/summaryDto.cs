using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class SummaryDto
    {
        public int ShopId { get; set; }
        public List<SummaryItem> Summary_Items { get; set; }
        public HourlySummary Hourly_Summary { get; set; }
        public PaymentSummary Payment_Summary { get; set; }
        public CustomDataGroup Custom_Data_Group { get; set; }
    }

    public class SummaryItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double Compare_Value { get; set; }
    }

    public class HourlySummary
    {
        public string Name { get; set; }
        public double[] Value { get; set; }
        public double[] Compare_Value { get; set; }
    }

    public class PaymentDetail
    {
        public decimal amount { get; set; }
        public string paymenttype { get; set; }
    }

    public class PaymentSummary
    {
        public string Name { get; set; }
        public List<PaymentDetail> Value { get; set; }
        public List<PaymentDetail> Compare_Value { get; set; }
    }

    public class CustomDataItem
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
    }

    public class CustomDataGroup
    {
        public string Name { get; set; }
        public List<CustomDataItem> Value { get; set; }
        public List<CustomDataItem> Compare_Value { get; set; }
    }
}
