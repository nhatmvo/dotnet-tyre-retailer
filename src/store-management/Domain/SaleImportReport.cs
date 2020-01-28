using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class SaleImportReport
    {
        public string ProductSaleId { get; set; }
        public string ProductImportId { get; set; }
        public int? Quantity { get; set; }

        public virtual ProductImport ProductImport { get; set; }
        public virtual ProductSale ProductSale { get; set; }
    }
}
