using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class SoldUnit
    {
        public string Id { get; set; }
        public string PriceFluctuationId { get; set; }
        public string WarrantyCode { get; set; }
        public string Type { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ReferPrice { get; set; }
        public int? Quantity { get; set; }
        public bool? Billing { get; set; }
        public DateTime? Datetime { get; set; }
        public string AccountId { get; set; }
        public string InvoiceLineId { get; set; }

        public virtual Account Account { get; set; }
        public virtual InvoiceLine InvoiceLine { get; set; }
        public virtual PriceFluctuation PriceFluctuation { get; set; }
    }
}
