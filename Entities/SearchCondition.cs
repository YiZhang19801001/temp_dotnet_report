using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class SearchCondition
  {
    public int ShopId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime Compare_DateFrom { get; set; }
    public DateTime Compare_DateTo { get; set; }
  }
}
