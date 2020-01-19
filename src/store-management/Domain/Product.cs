using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Product
    {
        public Product()
        {
            IeReport = new HashSet<IeReport>();
            PriceFluctuation = new HashSet<PriceFluctuation>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string Pattern { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int QuantityRemain { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyBy { get; set; }

        public virtual ICollection<IeReport> IeReport { get; set; }
        public virtual ICollection<PriceFluctuation> PriceFluctuation { get; set; }
    }
}
