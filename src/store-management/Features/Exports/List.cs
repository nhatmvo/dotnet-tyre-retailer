using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Features.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Exports
{
    public class List
    {
        public class Query : IRequest<ExportsEnvelope>
        {
            public Query()
            {

            }

            
            public Query (ProductsFilter filter)
            {
                Filter = filter;
            }

            public ProductsFilter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, ExportsEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<ExportsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<ProductExport> queryable = _context.ProductExport;

                if (request.Filter == null)
                {
                    queryable = queryable.Skip(0).Take(10);
                    var invoiceProducts = await queryable.Select(q => new ExportEnvelope {
                        NoBillRemainQuantity = q.NotBillRemainQuantity ?? 0,
                        ProductId = q.ProductId,
                        ProductName = q.Product.Name,
                        ProductPattern = q.Product.Pattern,
                        ProductSize = q.Product.Size,
                        ProductType = q.Product.Type
                    }).ToListAsync();
                    return new ExportsEnvelope
                    {
                        Products = invoiceProducts
                    };
                }

                if (!string.IsNullOrEmpty(request.Filter.Type))
                {
                    queryable = queryable.Where(p => p.Product.Type.Contains(request.Filter.Type));
                }
                if (!string.IsNullOrEmpty(request.Filter.Brand))
                {
                    queryable = queryable.Where(p => p.Product.Brand.Contains(request.Filter.Brand));
                }
                if (!string.IsNullOrEmpty(request.Filter.Size))
                {
                    queryable = queryable.Where(p => p.Product.Size.Contains(request.Filter.Size));
                }
                if (!string.IsNullOrEmpty(request.Filter.Pattern))
                {
                    queryable = queryable.Where(p => p.Product.Pattern.Contains(request.Filter.Pattern));
                }

                var exportValues = queryable
                    .Skip(request.Filter.Offset ?? 0)
                    .Take(request.Filter.Limit ?? 10)
                    .AsNoTracking();

                var result = await queryable.Select(q => new ExportEnvelope {
                    NoBillRemainQuantity = q.NotBillRemainQuantity ?? 0,
                    ProductId = q.ProductId,
                    ProductName = q.Product.Name,
                    ProductPattern = q.Product.Pattern,
                    ProductSize = q.Product.Size,
                    ProductType = q.Product.Type
                }).ToListAsync(cancellationToken);

                return new ExportsEnvelope
                {
                    Products = result
                };

            }
        }

    }
}
