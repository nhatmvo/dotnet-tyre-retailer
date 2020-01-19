using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceLine = new HashSet<InvoiceLine>();
        }

        public string Id { get; set; }
        public int? InvoiceNo { get; set; }
        public DateTime? ExportDate { get; set; }
        public string Detail { get; set; }
        public decimal? Total { get; set; }
        public string AccountId { get; set; }
        public string CustomerId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<InvoiceLine> InvoiceLine { get; set; }
    }
}
