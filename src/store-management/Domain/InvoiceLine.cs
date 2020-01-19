using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class InvoiceLine
    {
        public InvoiceLine()
        {
            SoldUnit = new HashSet<SoldUnit>();
        }

        public string Id { get; set; }
        public string ProductName { get; set; }
        public decimal? ExportPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public string InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual ICollection<SoldUnit> SoldUnit { get; set; }
    }
}
