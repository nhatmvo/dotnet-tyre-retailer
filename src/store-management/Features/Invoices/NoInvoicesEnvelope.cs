using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
    public class NoInvoicesEnvelope
    {
        public string ProductId { get; set; }
        public string Type { get; set; }
        public string Pattern { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string NotBillingQuantity { get; set; }
    }
}
