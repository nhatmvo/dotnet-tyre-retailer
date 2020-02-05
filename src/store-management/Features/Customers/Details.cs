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

namespace store_management.Features.Customers
{
    public class Details
    {
        public class Query : IRequest<CustomerEnvelope>
        {
            public Query(string taxNumber)
            {
                TaxNumber = taxNumber;
                
            }
            public string TaxNumber { get; set; }

        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.TaxNumber).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, CustomerEnvelope>
        {
            private readonly StoreContext _context;

            public QueryHandler(StoreContext context)
            {
                _context = context;
            }

            public async Task<CustomerEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.TaxCode.Equals(request.TaxNumber), cancellationToken);
                
                if (customer == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Customer = Constants.NOT_FOUND });
                }

                return new CustomerEnvelope(customer);
                
            }
        }
    }
}
