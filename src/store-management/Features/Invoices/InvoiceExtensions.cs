using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
    public static class InvoiceExtensions
    {
        public static IQueryable<Invoice> GetAllData(this DbSet<Invoice> invoices)
        {
            return invoices.Include(i => i.InvoiceLine);
        }
    }
}
