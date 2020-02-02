using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class ProductExport
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int? NotBillRemainQuantity { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
