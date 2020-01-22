using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Sale
{
    public class SaleEnvelope
    {

        public SaleEnvelope(List<SaleUnit> saleUnits)
        {
            SaleUnits = saleUnits;
        }

        public SaleEnvelope(DateTime saleDatetime, List<SaleUnit> saleUnits)
        {
            SaleUnits = saleUnits;
            SaleDatetime = saleDatetime;
        }
        public List<SaleUnit> SaleUnits { get; set; }
        public DateTime SaleDatetime { get; set; }
    }

    public class SalesEnvelope
    {
        public SalesEnvelope(List<Transaction> transactions)
        {
            Transactions = transactions;
        }

        public List<Transaction> Transactions { get; set; }
    }
}
