using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Docket
  {
    [Key]
    public int docket_id { get; set; }
    public decimal discount { get; set; }
    public decimal total_inc { get; set; }
    public DateTime docket_date { get; set; }
    public int original_id { get; set; }
  }
}
