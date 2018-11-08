using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Staff
  {
    [Key]
    public int staff_id { get; set; }
    public string surname { get; set; }
    public string given_names { get; set; }
  }
}
