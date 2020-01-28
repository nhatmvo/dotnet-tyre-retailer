using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Imports
{
    public class ImportEnvelope
    {
        public List<ProductImport> ProductImports { get; set; }
    }

    public class ImportsEnvelope
    {
        public List<Transaction> Transactions { get; set; }

        public ImportsEnvelope(List<Transaction> transactions)
        {
            Transactions = transactions;
        }
    }
}
