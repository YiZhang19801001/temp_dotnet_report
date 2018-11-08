using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.ViewModels
{
    public class CustomerCreateViewModel
    {
        [Required]
        public string user_name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(password))]
        public string confirm_password { get; set; }
    }
}
