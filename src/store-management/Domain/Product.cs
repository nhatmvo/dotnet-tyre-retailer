using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_management.Domain
{
    public partial class Product
    {
        public Product()
        {
            ProductImport = new HashSet<ProductImport>();
            ProductSale = new HashSet<ProductSale>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string Pattern { get; set; }
        public string ImagePath { get; set; }
        public decimal? RefPrice { get; set; }
        [NotMapped]
        public int NoBillRemainQuantity { get; set; }
        [NotMapped]
        public int RemainQuantity { get; set; }
        [JsonIgnore]
        public int TotalQuantity { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        [JsonIgnore]
        public virtual ICollection<InvoiceLine> InvoiceLine { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductImport> ProductImport { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductSale> ProductSale { get; set; }
    }
}
