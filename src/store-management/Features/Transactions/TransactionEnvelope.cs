using store_management.Domain;
using store_management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Transactions
{
    public class TransactionsEnvelope
    {
        public TransactionsEnvelope(List<SoldUnit> soldUnits) 
        {
            SoldUnits = soldUnits;
        }
        public List<SoldUnit> SoldUnits { get; set; }

    }

    public class TransactionEnvelope
    {
        public TransactionEnvelope(SoldUnit soldUnit)
        {
            SoldUnit = soldUnit;
        }
        public SoldUnit SoldUnit { get; set; }
    }

    public class TransactionFilter
    {

        public bool? IsBilling { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Type { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

}
