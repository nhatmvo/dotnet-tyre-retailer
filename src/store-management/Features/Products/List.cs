using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                    result = result.Select(p => {
                        p.RemainQuantity = _context.ProductImport.Where(pi => pi.ProductId.Equals(p.Id)).Sum(pi => pi.RemainQuantity).GetValueOrDefault();
                        p.NoBillRemainQuantity = _context.ProductImport.Where(pi => pi.ProductId.Equals(p.Id)).Sum(pi => pi.ExportableAmount).GetValueOrDefault();
                        return p;
                    }).ToList();
                    return new ProductsEnvelope {
                        Products = result,
                        ProductsCount = result.Count(),
                        AvailableFilter = GetAvailableFilter(request.Filter)
                    };
                } 

                if (!string.IsNullOrEmpty(request.Filter.Pattern))
                {
                    queryable = queryable.Where(q => separateFilterOptions(request.Filter.Pattern.ToLower()).Contains(q.Pattern.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.Filter.Brand))
                {
                    queryable = queryable.Where(q => separateFilterOptions(request.Filter.Brand.ToLower()).Contains(q.Pattern.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.Filter.Type))
                {
                    queryable = queryable.Where(q => separateFilterOptions(request.Filter.Type.ToLower()).Contains(q.Pattern.ToLower()));
                }
                if (!string.IsNullOrEmpty(request.Filter.Size))
                {
                    queryable = queryable.Where(q => separateFilterOptions(request.Filter.Size.ToLower()).Contains(q.Pattern.ToLower()));
                }                

                var products = await queryable
                    .Skip(request.Filter.PageIndex != null && request.Filter.PageSize != null ? request.Filter.PageIndex.Value * request.Filter.PageSize.Value : 0)
                    .Take(request.Filter.PageSize ?? 10)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                products = products.Select(p => { 
                    p.RemainQuantity = _context.ProductImport.Where(pi => pi.ProductId.Equals(p.Id)).Sum(pi => pi.RemainQuantity).GetValueOrDefault();
                    return p; 
                }).ToList();

                products = products.Where(p => p.NoBillRemainQuantity > request.Filter.NoBillQuantityGt).ToList();

                return new ProductsEnvelope()
                {
                    Products = products,
                    ProductsCount = total,
                    AvailableFilter = GetAvailableFilter(request.Filter)
                };
            }

            private List<String> separateFilterOptions(String options)
            {
                return options.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
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
                        queryable = queryable.Where(p => pFilter.Type.ToLower().Contains(p.Type.ToLower()));
                    if (!string.IsNullOrEmpty(pFilter.Brand))
                        queryable = queryable.Where(p => pFilter.Brand.ToLower().Contains(p.Brand.ToLower()));
                    if (!string.IsNullOrEmpty(pFilter.Pattern))
                        queryable = queryable.Where(p => pFilter.Pattern.ToLower().Contains(p.Pattern.ToLower()));
                    if (!string.IsNullOrEmpty(pFilter.Size))
                        queryable = queryable.Where(p => pFilter.Size.ToLower().Contains(p.Size.ToLower()));

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
