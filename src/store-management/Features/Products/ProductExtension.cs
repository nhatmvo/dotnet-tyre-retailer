using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public static class ProductExtension
    {
        public static IQueryable<Product> GetAllData(this DbSet<Product> products)
        {
            var listProds = products.Include(p => p.PriceFluctuation);
            foreach (var product in listProds)
            {
                var priceFlucVal = product.PriceFluctuation.OrderByDescending(pf => pf.Date).FirstOrDefault();
                if (priceFlucVal != null)
                    product.Price = priceFlucVal.ChangedImportPrice;
                else product.Price = 0;
            }
            return listProds;
        }

    }
}
