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

    public class InvoicesEnvelope
    {
        public InvoicesEnvelope(List<Invoice> invoices)
        {
            Invoices = invoices;
        }
        public List<Invoice> Invoices { get; set; }
    }
}
