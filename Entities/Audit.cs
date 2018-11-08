using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Audit
  {
    [Key]
    public int audit_id { get; set; }
    public string tran_type { get; set; }
    public double stock_id { get; set; }
    public int source_id { get; set; }
    public double movement { get; set; }
    public DateTime audit_date { get; set; }
  }
}
