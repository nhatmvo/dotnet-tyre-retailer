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

namespace store_management.Features.Sale
{
    public class Details
    {
        public class Query : IRequest<SaleEnvelope>
        {
            public Query(string id)
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

        public class Handler : IRequestHandler<Query, SaleEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<SaleEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var validator = (new QueryValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var transaction = await _context.Transaction
                        .Include(t => t.ProductSale)
                        .FirstOrDefaultAsync(t => t.Id.Equals(request.Id), cancellationToken);
                    return new SaleEnvelope(transaction.Date.Value, transaction.ProductSale.ToList());
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { Error = "Dữ liệu đầu vào không hợp lệ" });
            }
        }
    }
}
