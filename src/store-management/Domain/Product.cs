using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Product
    {
        public Product()
        {
            TxReport = new HashSet<TxReport>();
        }

        public byte[] Id { get; set; }
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
        public byte[] CreatedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public byte[] ModifyBy { get; set; }

        public virtual ICollection<TxReport> TxReport { get; set; }
    }
}
