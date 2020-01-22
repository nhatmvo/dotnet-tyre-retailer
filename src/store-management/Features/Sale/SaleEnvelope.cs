using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Sale
{
    public class SaleEnvelope
    {

        public SaleEnvelope(List<SoldUnit> soldUnits)
        {
            SoldUnits = soldUnits;
        }
        public List<SoldUnit> SoldUnits { get; set; }
    }
}
