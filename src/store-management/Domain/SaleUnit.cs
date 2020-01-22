using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class SaleUnit
    {
        public string Id { get; set; }
        public string PriceFluctuationId { get; set; }
        public string WarrantyCode { get; set; }
        public string Type { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ReferPrice { get; set; }
        public int? Quantity { get; set; }
        public bool? Billing { get; set; }
        public string TransactionId { get; set; }

        public virtual PriceFluctuation PriceFluctuation { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
