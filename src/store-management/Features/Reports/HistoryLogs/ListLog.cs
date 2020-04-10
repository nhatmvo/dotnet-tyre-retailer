using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Logs
{
    public class ListLog
    {
        public class Query : IRequest<LogsEnvelope>
        {
            public LogFilter Filter { get; set; }
            public Query() { }

            public Query(LogFilter filter)
            {
                Filter = filter;
            }
        }

        public class Handler : IRequestHandler<Query, LogsEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<LogsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.OperationHistory.AsQueryable();
                if (request.Filter.StartDate != null)
                {
                    queryable = queryable.Where(q => q.ActionDate >= request.Filter.StartDate.Value);
                }

                if (request.Filter.EndDate != null)
                {
                    queryable = queryable.Where(q => q.ActionDate <= request.Filter.EndDate.Value);
                }

                var logs = await queryable
                        .Skip(request.Filter.Offset ?? 0)
                        .Take(request.Filter.Limit ?? 30)
                        .AsNoTracking().ToListAsync(cancellationToken);
                return new LogsEnvelope(logs);
            }
        }
    }
}
