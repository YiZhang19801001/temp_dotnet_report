using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demoBusinessReport.Entities;

namespace demoBusinessReport.ViewModels
{
    public class CustomerLinkViewModel
    {
        public IEnumerable<Customer> customer_list { get; set; }
        public IEnumerable<ShopShortInfo> shop_info_list { get; set; }
        public string customer_id { get; set; }
    }

    public class Customer {
        public string customer_id { get; set; }
        public string customer_name { get; set; }
    }
    public class ShopShortInfo {
        public int shop_id { get; set; }
        public string shop_name { get; set; }
        public Boolean isPicked { get; set; }
    }
}
