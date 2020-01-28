using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Imports
{
    public class Details
    {
        public class Query : IRequest<ImportEnvelope>
        {
            public string Id { get; set; }
            public Query (string id)
            {
                Id = id;
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, ImportEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<ImportEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var validator = (new QueryValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var transaction = await _context.Transaction
                        .Include(t => t.ProductImport)
                        .FirstOrDefaultAsync(t => t.Id.Equals(request.Id), cancellationToken);
                    return new ImportEnvelope
                    {
                        ProductImports = transaction.ProductImport.ToList()
                    };
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });
            }
        }
    }
}
