using demoBusinessReport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Dtos
{
    public class ShopsDto
    {
        public IEnumerable<Shop> shops { get; set; }
    }
}
