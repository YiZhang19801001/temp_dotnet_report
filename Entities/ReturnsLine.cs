using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace demoBusinessReport.Entities
{
  public class ReturnsLine
  {
    [Key]
    public int line_id { get; set; }
    public int quantity { get; set; }
        public int returns_id { get; set; }
    }
}
