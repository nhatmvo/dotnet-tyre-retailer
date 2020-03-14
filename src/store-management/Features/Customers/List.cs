using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Customers
{
    public class List
    {
        public class CustomerFilter
        {
            public string TaxCode { get; set; }
            public string Name { get; set; }
            public string BankAccountNum { get; set; }
            public int? Limit { get; set; }
            public int? Offset { get; set; }
        }

        public class Query : IRequest<CustomersEnvelope>
        {
            public CustomerFilter Filter;
        }

        public class QueryHandler : IRequestHandler<Query, CustomersEnvelope>
        {
            private readonly StoreContext _context; 

            public QueryHandler(StoreContext context)
            {
                _context = context;
            }

            public async Task<CustomersEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Customer.AsQueryable();

                if (request.Filter != null)
                {
                    if (!string.IsNullOrEmpty(request.Filter.Name))
                    {
                        queryable = queryable.Where(q => q.FullName.Contains(request.Filter.Name));
                    }
                    if (!string.IsNullOrEmpty(request.Filter.TaxCode))
                    {
                        queryable = queryable.Where(q => q.TaxCode.Contains(request.Filter.TaxCode));
                    }
                    if (!string.IsNullOrEmpty(request.Filter.BankAccountNum))
                    {
                        queryable = queryable.Where(q => q.BankAccountNumber.Contains(request.Filter.BankAccountNum));
                    }
                    var customers = await queryable
                        .Skip(request.Filter.Offset ?? 0)
                        .Take(request.Filter.Limit ?? 10)
                        .AsNoTracking().ToListAsync(cancellationToken);
                    return new CustomersEnvelope(customers);
                }
                return new CustomersEnvelope(await queryable.Skip(0).Take(10)
                    .AsNoTracking().ToListAsync(cancellationToken));
            }
        }

    }
}
