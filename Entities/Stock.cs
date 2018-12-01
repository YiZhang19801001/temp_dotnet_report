using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Stock
  {
    [Key]
    public double stock_id { get; set; }
    public string cat1 { get; set; }
    public string description { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
    }
}
