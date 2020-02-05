using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class ProductSale
    {
        public ProductSale()
        {
            SaleImportReport = new HashSet<SaleImportReport>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public string WarrantyCode { get; set; }
        public string Type { get; set; }
        public decimal? SalePrice { get; set; }
        public int? SaleAmount { get; set; }
        public string TransactionId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [JsonIgnore]
        public virtual Transaction Transaction { get; set; }
        [JsonIgnore]
        public virtual ICollection<SaleImportReport> SaleImportReport { get; set; }
    }
}
