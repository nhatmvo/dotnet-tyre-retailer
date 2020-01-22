using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class PriceFluctuation
    {
        public PriceFluctuation()
        {
            SaleUnit = new HashSet<SaleUnit>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public DateTime? Date { get; set; }
        public decimal ChangedImportPrice { get; set; }
        public decimal? CurrentImportPrice { get; set; }
        public decimal ChangedPrice { get; set; }
        public decimal? CurrentPrice { get; set; }

        public virtual Product Product { get; set; }
        public virtual ICollection<SaleUnit> SaleUnit { get; set; }
    }
}
