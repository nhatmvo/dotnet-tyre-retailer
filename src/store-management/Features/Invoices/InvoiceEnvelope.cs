using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
    public class InvoiceEnvelope
    {
        public InvoiceEnvelope(Invoice invoice)
        {
            Invoice = invoice;
        }
        public Invoice Invoice { get; set; }
    }
}
