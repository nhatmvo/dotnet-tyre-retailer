using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Sale
{
    public class List
    {
        public class TransactionFilter
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? PageIndex { get; set; }
            public int? PageSize { get; set; }
        }

        public class Query : IRequest<SalesEnvelope>
        { 
            public TransactionFilter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, SalesEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<SalesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Transaction.Where(t => t.Type.Equals(TransactionType.SOLD))
                    .Include(t => t.ProductSale);
                
                if (request.Filter != null)
                {
                    if (request.Filter.FromDate.HasValue)
                        queryable.Where(t => t.Date >= request.Filter.FromDate);
                    if (request.Filter.ToDate.HasValue)
                        queryable.Where(t => t.Date <= request.Filter.ToDate);
                    var transactions = await queryable
                        .Skip(request.Filter.PageIndex != null && request.Filter.PageSize != null ? request.Filter.PageIndex.Value * request.Filter.PageSize.Value : 0)
                        .Take(request.Filter.PageSize ?? 10)
                        .AsNoTracking().ToListAsync(cancellationToken);
                    return new SalesEnvelope(transactions);
                }
                 ;
                return new SalesEnvelope(await queryable.Skip(0).Take(10)
                    .AsNoTracking().ToListAsync(cancellationToken));
            }
        }
    }
}
