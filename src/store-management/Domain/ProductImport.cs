using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_management.Domain
{
    public partial class ProductImport
    {
        public ProductImport()
        {
            SaleImportReport = new HashSet<SaleImportReport>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public string TransactionId { get; set; }
        public DateTime? Date { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal? CurrentImportPrice { get; set; }
        public int? ImportAmount { get; set; }
        public int? RemainQuantity { get; set; }
        [NotMapped]
        public int ProductTotalQuantity { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [JsonIgnore]
        public virtual Transaction Transaction { get; set; }
        [JsonIgnore]
        public virtual ICollection<SaleImportReport> SaleImportReport { get; set; }
    }
}
