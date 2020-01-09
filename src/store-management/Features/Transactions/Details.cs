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
                var invoice = await _context.Invoice.FirstOrDefaultAsync(i => request.Id.Equals((new Guid(i.Id)).ToString()));
                if (invoice == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Invoice = Constants.NOT_FOUND });
                return new TransactionEnvelope(invoice);
            }
        }


    }
}
