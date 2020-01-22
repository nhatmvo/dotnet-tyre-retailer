using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class InvoiceLine
    {
        public string Id { get; set; }
        public decimal? ExportPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
        public string InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
