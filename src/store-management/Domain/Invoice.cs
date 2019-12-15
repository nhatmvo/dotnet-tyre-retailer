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

        public byte[] Id { get; set; }
        public int? InvoiceNo { get; set; }
        public DateTime? ExportDate { get; set; }
        public string Detail { get; set; }
        public decimal? Total { get; set; }
        public int? Status { get; set; }
        public byte[] AccountId { get; set; }
        public byte[] CustomerId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<InvoiceLine> InvoiceLine { get; set; }
    }
}
