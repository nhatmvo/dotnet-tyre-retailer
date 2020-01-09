using store_management.Domain;
using store_management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Transactions
{
    public class TransactionEnvelope
    {
        public TransactionEnvelope(Invoice invoice) 
        {
            Invoice = invoice;
        }
        public Invoice Invoice { get; }

    }

    public enum InvoiceTypes
    {
        BILLED = 1,
        NON_BILLED = 2
    }
}
