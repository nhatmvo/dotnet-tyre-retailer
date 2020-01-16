using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Transactions
{
    public class Details
    {
        public class Query : IRequest<TransactionEnvelope>
        {
            public Query (string id)
            {
                Id = id;
            }
            public string Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, TransactionEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<TransactionEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var exportUnit = await _context.ExportUnit.FromSqlRaw($"SELECT * FROM EXPORT_UNIT WHERE ID = {(new Guid(request.Id)).ToByteArray()}").FirstOrDefaultAsync();
                if (exportUnit != null)
                {
                    return new TransactionEnvelope(exportUnit);
                }
                else throw new RestException(HttpStatusCode.NotFound, new { });
                
            }
        }


    }
}
