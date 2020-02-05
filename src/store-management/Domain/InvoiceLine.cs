using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_management.Domain
{
    public partial class InvoiceLine
    {
        public string Id { get; set; }
        public decimal? ExportPrice { get; set; }
        public int? ExportAmount { get; set; }
        public decimal? Total { get; set; }
        public string InvoiceId { get; set; }
        public string ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [JsonIgnore]
        public virtual Invoice Invoice { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
    }
}
