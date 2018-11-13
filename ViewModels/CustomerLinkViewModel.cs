using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demoBusinessReport.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace demoBusinessReport.ViewModels
{
    public class CustomerLinkViewModel
    {
        public List<SelectListItem> customer_list { get; set; }
        public List<ShopShortInfo> shop_info_list { get; set; }
        public string customer_id { get; set; }
    }


    public class ShopShortInfo {
        public int shop_id { get; set; }
        public string shop_name { get; set; }
        public Boolean isPicked { get; set; }
    }
}
