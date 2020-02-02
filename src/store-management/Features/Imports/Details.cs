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
                    var productImports = _context.ProductImport
                        .Where(t => t.Product.Id.Equals(request.Id));
                    return new ImportEnvelope
                    {
                        ProductImports = await productImports.ToListAsync(cancellationToken)
                    };
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });
            }
        }
    }
}
