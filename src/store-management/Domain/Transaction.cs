using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Transaction
    {
        public Transaction()
        {
            ImportUnit = new HashSet<ImportUnit>();
            SaleUnit = new HashSet<SaleUnit>();
        }

        public string Id { get; set; }
        public DateTime? Date { get; set; }
        public string AccountId { get; set; }
        public string Type { get; set; }
        public bool? Billing { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<ImportUnit> ImportUnit { get; set; }
        public virtual ICollection<SaleUnit> SaleUnit { get; set; }
    }
}
