using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class InvoiceLine
    {
        public byte[] Id { get; set; }
        public byte[] ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public byte[] InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
    }
}
