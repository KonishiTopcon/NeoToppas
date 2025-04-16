using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoToppas.Domain.Entities
{
    public class ShippingListEntities
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int ShippingCnt { get; set; }
        public string Bikou { get; set; }
        public int Page { get; set; }
        public int RegisteredCnt { get; set; }
    }
}
