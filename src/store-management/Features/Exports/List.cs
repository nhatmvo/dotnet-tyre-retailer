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
                        NoBillRemainQuantity = q.NoBillRemainQuantity ?? 0,
                        Id = q.Id,
                        Name = q.ProductImport.Product.Name,
                        Pattern = q.ProductImport.Product.Pattern,
                        Size = q.ProductImport.Product.Size,
                        Type = q.ProductImport.Product.Type,
                        Brand = q.ProductImport.Product.Brand,
                        RefPrice = q.ProductImport.Product.RefPrice
                    }).ToListAsync();
                    return new ExportsEnvelope
                    {
                        Products = invoiceProducts
                    };
                }

                if (!string.IsNullOrEmpty(request.Filter.Type))
                {
                    queryable = queryable.Where(p => p.ProductImport.Product.Type.Contains(request.Filter.Type));
                }
                if (!string.IsNullOrEmpty(request.Filter.Brand))
                {
                    queryable = queryable.Where(p => p.ProductImport.Product.Brand.Contains(request.Filter.Brand));
                }
                if (!string.IsNullOrEmpty(request.Filter.Size))
                {
                    queryable = queryable.Where(p => p.ProductImport.Product.Size.Contains(request.Filter.Size));
                }
                if (!string.IsNullOrEmpty(request.Filter.Pattern))
                {
                    queryable = queryable.Where(p => p.ProductImport.Product.Pattern.Contains(request.Filter.Pattern));
                }

                var exportValues = queryable
                    .Skip(request.Filter.Offset ?? 0)
                    .Take(request.Filter.Limit ?? 10)
                    .AsNoTracking();

                var result = await queryable.Select(q => new ExportEnvelope {
                    NoBillRemainQuantity = q.NoBillRemainQuantity ?? 0,
                    Id = q.Id,
                    Name = q.ProductImport.Product.Name,
                    Pattern = q.ProductImport.Product.Pattern,
                    Size = q.ProductImport.Product.Size,
                    Type = q.ProductImport.Product.Type,
                    Brand = q.ProductImport.Product.Brand,
                    RefPrice = q.ProductImport.Product.RefPrice
                }).ToListAsync(cancellationToken);

                return new ExportsEnvelope
                {
                    Products = result
                };

            }
        }

    }
}
