using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Return
  {
    [Key]
    public int returns_id { get; set; }
    public decimal total_inc { get; set; }
        public DateTime returns_date { get; set; }
    }
}
