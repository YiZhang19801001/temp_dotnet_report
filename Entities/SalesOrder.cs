using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class SalesOrder
  {
    [Key]
    public int salesorder_id { get; set; }
    public string custom { get; set; }
    public DateTime salesorder_date { get; set; }
    public decimal total_inc { get; set; }
  }
}
