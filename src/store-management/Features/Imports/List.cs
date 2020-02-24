﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Imports
{
    public class List
    {
        public class ImportTransactionFilter
        {
            public string ProductId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? Limit { get; set; }
            public int? Offset { get; set; }
        }

        public class Query : IRequest<ImportsEnvelope>
        {
            public ImportTransactionFilter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, ImportsEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<ImportsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.ProductImport;

                if (request.Filter != null)
                {
                    if (request.Filter.FromDate.HasValue)
                        queryable.Where(t => t.Date >= request.Filter.FromDate);
                    if (request.Filter.ToDate.HasValue)
                        queryable.Where(t => t.Date <= request.Filter.ToDate);
                    if (!string.IsNullOrEmpty(request.Filter.ProductId))
                    {
                        queryable.Where(pi => pi.ProductId.Equals(request.Filter.ProductId));
                    }
                    var imports = await queryable
                        .Skip(request.Filter.Offset ?? 0)
                        .Take(request.Filter.Limit ?? 10)
                        .AsNoTracking().ToListAsync(cancellationToken);
                    return new ImportsEnvelope(imports);
                }
                 ;
                return new ImportsEnvelope(await queryable.Skip(0).Take(10)
                    .AsNoTracking().ToListAsync(cancellationToken));
            }
        }
    }
}
