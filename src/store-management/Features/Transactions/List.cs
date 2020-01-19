using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Features.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Transactions
{
    public class List
    {
        public class Query : IRequest<TransactionsEnvelope>
        {
            public Query()
            {

            }

            public Query(TransactionFilter filter)
            {
                Filter = filter;
            }

            public TransactionFilter Filter { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, TransactionsEnvelope>
        {
            private readonly StoreContext _context;
           
            public QueryHandler(StoreContext context)
            {
                _context = context;
            } 
            public async Task<TransactionsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.SoldUnit;
                if (request.Filter != null)
                {
                    if (request.Filter.IsBilling != null)
                        queryable.Where(q => q.Billing == request.Filter.IsBilling);
                    if (request.Filter.FromDate != null)
                        queryable.Where(q => q.Datetime > request.Filter.FromDate);
                    if (request.Filter.ToDate != null)
                        queryable.Where(q => q.Datetime < request.Filter.ToDate);
                } else
                {
                    queryable.OrderByDescending(su => su.Datetime);
                }

                var transactions = await queryable
                    .Skip(request.Filter.Offset ?? 0)
                    .Take(request.Filter.Limit ?? 10)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new TransactionsEnvelope(transactions);
            }
        }


    }
}
