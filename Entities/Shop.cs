using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Entities
{
  public class Shop
  {
    [Key]
    public int shop_id { get; set; }
    //public string Server { get; set; }
    //public string Database { get; set; }
    public string shop_name { get; set; }
    public string db_path { get; set; }
    public string db_password { get; set; }
    }
}
