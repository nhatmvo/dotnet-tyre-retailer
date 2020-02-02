using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace store_management.Domain
{
    public partial class Transaction
    {
        public Transaction()
        {
            ProductImport = new HashSet<ProductImport>();
            ProductSale = new HashSet<ProductSale>();
        }

        public string Id { get; set; }
        public DateTime? Date { get; set; }
        public string AccountId { get; set; }
        public string Type { get; set; }
        public bool? Billing { get; set; }
        [JsonIgnore]
        public virtual Account Account { get; set; }
        public virtual ICollection<ProductImport> ProductImport { get; set; }
        public virtual ICollection<ProductSale> ProductSale { get; set; }
    }
}
