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
            return products.Include(p => p.ProductImport.OrderByDescending(pi => pi.Date));
        }

    }
}
