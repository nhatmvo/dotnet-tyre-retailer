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
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
        public string InvoiceId { get; set; }
        [JsonIgnore]
        public virtual Invoice Invoice { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
    }
}
