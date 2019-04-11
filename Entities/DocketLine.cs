using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class DocketLine
  {
    [Key]
    public int line_id { get; set; }
    public int docket_id { get; set; }
    public double stock_id { get; set; }
    public double quantity { get; set; }
    public decimal sell_inc { get; set; }
    public Int16? size_level { get; set; }
  }
}
