using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
    public class List
    {
        public class InvoiceFilter
        {
            public InvoiceFilter(DateTime fromDate, DateTime toDate)
            {
                FromDate = fromDate;
                ToDate = toDate;
            }

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public int? Offset { get; set; }
            public int? Limit { get; set; }
        }

        public class Query : IRequest<InvoicesEnvelope>
        {
            public InvoiceFilter Filter { get; set; }

            public Query()
            {

            }

            public Query(InvoiceFilter filter)
            {
                Filter = filter;
            }
        }

        public class Handler : IRequestHandler<Query, InvoicesEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<InvoicesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Invoice.GetAllData();
                
                if (request.Filter == null)
                {
                    var result = await queryable.ToListAsync();
                    return new InvoicesEnvelope(result);

                }
                if (request.Filter.FromDate != null)
                    queryable = queryable.Where(q => q.ExportDate >= request.Filter.FromDate);
                if (request.Filter.ToDate != null)
                    queryable = queryable.Where(q => q.ExportDate <= request.Filter.ToDate);
                var invoices = await queryable
                    .Skip(request.Filter.Offset ?? 0)
                    .Take(request.Filter.Limit ?? 10)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new InvoicesEnvelope(invoices);
            }
        }
    }
}
