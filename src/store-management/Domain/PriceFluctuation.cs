using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class PriceFluctuation
    {
        public PriceFluctuation()
        {
            ExportUnit = new HashSet<ExportUnit>();
        }

        public byte[] Id { get; set; }
        public byte[] ProductId { get; set; }
        public DateTime? Date { get; set; }
        public decimal ChangedImportPrice { get; set; }
        public decimal? CurrentImportPrice { get; set; }
        public decimal ChangedPrice { get; set; }
        public decimal? CurrentPrice { get; set; }

        public virtual ICollection<ExportUnit> ExportUnit { get; set; }
    }
}
