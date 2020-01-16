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
        public TransactionsEnvelope(List<ExportUnit> exportUnits) 
        {
            ExportUnits = exportUnits;
        }
        public List<ExportUnit> ExportUnits { get; set; }

    }

    public class TransactionEnvelope
    {
        public TransactionEnvelope(ExportUnit exportUnit)
        {
            ExportUnit = exportUnit;
        }
        public ExportUnit ExportUnit { get; set; }
    }

    public enum InvoiceTypes
    {
        BILLED = 1,
        NON_BILLED = 2
    }
}
