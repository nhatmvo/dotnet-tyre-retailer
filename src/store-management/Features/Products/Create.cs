using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public class Create
    {
        public class ProductData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Size { get; set; }
            public string Brand { get; set; }
            public string Pattern { get; set; }
            public string ImagePath { get; set; }
            public decimal Price { get; set; }
            public decimal ImportPrice { get; set; }
            public int Quantity { get; set; }
            public string Description { get; set; }
        }

        public class ProductDataValidator : AbstractValidator<ProductData>
        {
            public ProductDataValidator()
            {
                RuleFor(x => x.Name).NotEmpty().NotNull();
                RuleFor(x => x.Type).NotEmpty().NotNull();
                RuleFor(x => x.Size).NotEmpty().NotNull();
                RuleFor(x => x.Brand).NotEmpty().NotNull();
                RuleFor(x => x.Pattern).NotEmpty().NotNull();
                RuleFor(x => x.Price).NotEmpty().NotNull().GreaterThan(0);
                RuleFor(x => x.ImportPrice).NotEmpty().NotNull().GreaterThan(0);
                RuleFor(x => x.Quantity).NotEmpty().NotNull().GreaterThan(0);
            }
        }

        public class Command : IRequest<ProductsEnvelope>
        {
            public List<ProductData> ProductsData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.ProductsData).SetValidator(new ProductDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, ProductsEnvelope>
        {

            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ProductsEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var insertProducts = new List<Product>();
                    var productsPriceFluctuation = new List<PriceFluctuation>();
                    var exportImportReports = new List<IeReport>();
                    var now = DateTime.Now;
                    
                    foreach (var item in request.ProductsData)
                    {
                        // check if insert product with 4 properties already existed
                        var product = await _context.Product.FirstOrDefaultAsync(p => p.Type.Equals(item.Type)
                            && p.Name.Equals(item.Name) && p.Brand.Equals(item.Brand) && p.Pattern.Equals(item.Pattern));
                        if (product != null)
                            throw new RestException(HttpStatusCode.BadRequest, new { });
                        var productId = Guid.NewGuid().ToString();
                        var productToCreate = new Product
                        {
                            Id = productId,
                            Name = item.Name,
                            Type = item.Type,
                            Brand = item.Brand,
                            Pattern = item.Pattern,
                            Price = item.Price,
                            QuantityRemain = item.Quantity,
                            Description = item.Description,
                            CreatedDate = now
                            // Add created person
                        };
                        insertProducts.Add(productToCreate);

                        var priceFluctuation = new PriceFluctuation
                        {
                            Id = Guid.NewGuid().ToString(),
                            ChangedImportPrice = item.ImportPrice,
                            ChangedPrice = item.Price,
                            Date = now,
                            ProductId = productId
                        };
                        productsPriceFluctuation.Add(priceFluctuation);

                        var exportImportReport = new IeReport
                        {
                            ProductId = productId,
                            CreateTime = now,
                            TotalQuantity = 1 * item.Quantity,
                            TotalPrice = -1 * item.Quantity * item.ImportPrice
                        };
                        exportImportReports.Add(exportImportReport);
                    }
                    // Add list product
                    await _context.Product.AddRangeAsync(insertProducts);
                    // First init price fluctuation for inserted Product
                    await _context.PriceFluctuation.AddRangeAsync(productsPriceFluctuation);
                    await _context.IeReport.AddRangeAsync(exportImportReports);
                    await _context.SaveChangesAsync();
                    return new ProductsEnvelope {
                        Products = insertProducts, 
                        ProductsCount = insertProducts.Count
                    };
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });


            }
        }
    }
}
