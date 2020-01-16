using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public class ProductsEnvelope
    {
        public List<Product> Products { get; set; }
        public int ProductsCount { get; set; }
    }

    public class ProductsFilter
    {
        public string Type { get; set; }
        public string Size { get; set; }
        public string Branch { get; set; }
        public string Pattern { get; set; }
        public bool Newest { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }

    }
}
