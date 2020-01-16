using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class ExportUnit
    {
        public byte[] Id { get; set; }
        public byte[] PriceFluctuationId { get; set; }
        public string WarrantyCode { get; set; }
        public string Type { get; set; }
        public decimal? ExportPrice { get; set; }
        public int? Quantity { get; set; }
        public bool? Billing { get; set; }
        public DateTime? ExportDatetime { get; set; }
        public byte[] InvoiceLineId { get; set; }

        public virtual InvoiceLine InvoiceLine { get; set; }
        public virtual PriceFluctuation PriceFluctuation { get; set; }
    }
}
