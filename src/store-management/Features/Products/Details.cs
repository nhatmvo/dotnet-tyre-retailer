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
            public Query(string id, ProductData productData = null)
            {
                Id = id;
                ProductData = productData;
            }
            public string Id { get; set; }
            public ProductData ProductData { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                //RuleFor(x => x.Id).NotNull().NotEmpty().Unless(x => x.ProductData == null);
                //RuleFor(x => x.ProductData).NotNull().Unless(x => string.IsNullOrEmpty(x.Id));
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
                var validationResult = (new QueryValidator()).Validate(request);
                
                if (validationResult.IsValid)
                {
                    var product = new Product();
                    if (!string.IsNullOrEmpty(request.Id))
                    {
                        product = await _context.Product
                            .FirstOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);
                        if (product == null)
                            throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });
                    }
                    else if (request.ProductData != null)
                    {
                        product = await _context.Product
                            .FirstOrDefaultAsync(p => p.Brand.Equals(request.ProductData.Brand)
                            && p.Pattern.Equals(request.ProductData.Pattern)
                            && p.Size.Equals(request.ProductData.Size)
                            && p.Type.Equals(request.ProductData.Type));
                        if (product == null)
                            throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });
                    }

                    return new ProductEnvelope(product);
                } else
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { });
                }
                
                
            }
        }




    }
}
