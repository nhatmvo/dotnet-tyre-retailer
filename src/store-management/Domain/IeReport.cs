using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class IeReport
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string ProductId { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Action { get; set; }
        public int? QuantityUpdate { get; set; }
        public decimal? PriceUpdate { get; set; }

        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }
    }
}
