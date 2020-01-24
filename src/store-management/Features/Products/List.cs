using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public class List
    {
        public class Query : IRequest<ProductsEnvelope>
        {
            public Query()
            {

            }

            public Query(ProductsFilter filter)
            {
                Filter = filter;
            }

            public ProductsFilter Filter { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, ProductsEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public QueryHandler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }


            public async Task<ProductsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<Product> queryable = _context.Product.GetAllData();
                int total = queryable.Count();

                if (request.Filter == null) {
                    var result = await queryable.ToListAsync();
                    return new ProductsEnvelope {
                        Products = result,
                        ProductsCount = result.Count(),
                        FilterEnvelope = GetAvailableFilter(request.Filter)
                    };
                } 

                if (!string.IsNullOrEmpty(request.Filter.Pattern))
                {
                    queryable = queryable.Where(q => q.Pattern.Contains(request.Filter.Pattern));
                }

                if (!string.IsNullOrEmpty(request.Filter.Branch))
                {
                    queryable = queryable.Where(q => q.Brand.Contains(request.Filter.Branch));
                }

                if (!string.IsNullOrEmpty(request.Filter.Type))
                {
                    queryable = queryable.Where(q => q.Type.Equals(request.Filter.Type));
                }

                
                var products = await queryable
                    .Skip(request.Filter.Offset ?? 0)
                    .Take(request.Filter.Limit ?? 10)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new ProductsEnvelope()
                {
                    Products = products,
                    ProductsCount = total,
                    FilterEnvelope = GetAvailableFilter(request.Filter)
                };
            }

            private FilterEnvelope GetAvailableFilter(ProductsFilter pFilter)
            {
                var queryable = _context.Product.AsQueryable();
                if (pFilter == null)
                {
                    return new FilterEnvelope
                    {
                        Brands = queryable.Select(p => p.Brand).Distinct().ToList(),
                        Types = queryable.Select(p => p.Type).Distinct().ToList(),
                        Patterns = queryable.Select(p => p.Pattern).Distinct().ToList(),
                        Sizes = queryable.Select(p => p.Size).Distinct().ToList()
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(pFilter.Type)) 
                        queryable = queryable.Where(p => p.Type.Equals(pFilter.Type));
                    if (!string.IsNullOrEmpty(pFilter.Branch))
                        queryable = queryable.Where(p => p.Brand.Equals(pFilter.Branch));
                    if (!string.IsNullOrEmpty(pFilter.Pattern))
                        queryable = queryable.Where(p => p.Pattern.Equals(pFilter.Pattern));
                    if (!string.IsNullOrEmpty(pFilter.Size))
                        queryable = queryable.Where(p => p.Size.Equals(pFilter.Size));

                    return new FilterEnvelope
                    {
                        Brands = queryable.Select(p => p.Brand).Distinct().ToList(),
                        Types = queryable.Select(p => p.Type).Distinct().ToList(),
                        Patterns = queryable.Select(p => p.Pattern).Distinct().ToList(),
                        Sizes = queryable.Select(p => p.Size).Distinct().ToList()
                    };

                }
                        
                

        }
        }

        
    }
}
