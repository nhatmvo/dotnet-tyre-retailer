using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public class ProductEnvelope
    {
        public ProductEnvelope(Product product)
        {
            Product = product;
        }
        public Product Product { get; }
    }

}
