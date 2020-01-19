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

namespace store_management.Features.Products
{
    public class Details
    {

        public class ProductData
        {
            public string Pattern { get; set; }
            public string Type { get; set; }
            public string Brand { get; set; }
            public string Size { get; set; }
        }

        public class Query : IRequest<ProductEnvelope>
        {
            public Query(string id)
            {
                Id = id;
            }
            public string Id { get; set; }
            public ProductData ProductData { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Id).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<Query, ProductEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ProductEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var product = await _context.Product
                    .Include(p => p.PriceFluctuation)
                    .FirstOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

                if (product == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });

                return new ProductEnvelope(product);
                
            }
        }




    }
}
