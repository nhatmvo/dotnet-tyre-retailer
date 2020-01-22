using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class ImportUnit
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? ImportPrice { get; set; }
        public string TransactionId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
