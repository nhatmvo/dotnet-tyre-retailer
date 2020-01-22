using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Features.Importation;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Importations
{
    public class Create
    {
        public class ImportData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Size { get; set; }
            public string Brand { get; set; }
            public string Pattern { get; set; }
            public decimal Price { get; set; }
            public decimal ImportPrice { get; set; }
            public int Quantity { get; set; }
        }

        public class ImportDataValidator : AbstractValidator<ImportData>
        {
            public ImportDataValidator()
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

        public class Command : IRequest<ImportsEnvelope>
        {
            public List<ImportData> ImportsData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.ImportsData).SetValidator(new ImportDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, ImportsEnvelope>
        {

            private readonly StoreContext _context;
            private readonly DateTime _now;

            public Handler(StoreContext context)
            {
                _context = context;
                _now = DateTime.Now;
            }

            public async Task<ImportsEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var productsInsert = new List<Product>();
                    var productsUpdate = new List<Product>();
                    var priceFluctuationInsert = new List<PriceFluctuation>();
                    var ieReports = new List<IeReport>();

                    foreach (var item in request.ImportsData)
                    {
                        // for each product created or updated, the following tables is also inserted
                        //  1. Product - contains information about a Product
                        //  2. PriceFluctuation - updated with lastest price of a product
                        //  3. IeReport - using for export & import information when create report
                        var product = await GetProductByProps(item.Pattern, item.Type, item.Brand, item.Size);
                        string productId = string.Empty;
                        if (product == null)
                        {
                            productId = Guid.NewGuid().ToString();
                            productsInsert.Add(GetCreateProduct(productId, item));
                            priceFluctuationInsert.Add(GetProductPriceFlucCreate(productId, item.ImportPrice, item.Price));
                        }
                        else
                        {
                            productId = product.Id;
                            productsUpdate.Add(GetUpdateProduct(productId, item, product.QuantityRemain));
                            priceFluctuationInsert.Add(GetProductPriceFlucUpdate(productId, item, product));
                        }
                        ieReports.Add(new IeReport
                        {
                            Id = Guid.NewGuid().ToString(),
                            CreateTime = _now,
                            ProductId = productId,
                            TotalPrice = item.ImportPrice * item.Quantity,
                            TotalQuantity = item.Quantity
                        });
                        
                    }
                    await _context.Product.AddRangeAsync(productsInsert);
                    _context.Product.UpdateRange(productsUpdate);

                    await _context.PriceFluctuation.AddRangeAsync(priceFluctuationInsert);
                    await _context.IeReport.AddRangeAsync(ieReports);

                    await _context.SaveChangesAsync();

                    return new ImportsEnvelope
                    {
                        Products = productsInsert.Concat(productsUpdate).ToList()
                    };
                } else
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { });
                }
            }

            private PriceFluctuation GetProductPriceFlucUpdate(string productId, ImportData data, Product existedProduct)
            {
                return new PriceFluctuation
                {
                    Id = Guid.NewGuid().ToString(),
                    ChangedImportPrice = data.ImportPrice,
                    ChangedPrice = data.Price,
                    CurrentImportPrice = existedProduct.PriceFluctuation.FirstOrDefault().ChangedImportPrice,
                    CurrentPrice = existedProduct.Price,
                    ProductId = productId
                };
            }


            private PriceFluctuation GetProductPriceFlucCreate(string productId, decimal importPrice, decimal refPrice)
            {
                return new PriceFluctuation
                {
                    Id = Guid.NewGuid().ToString(),
                    ChangedPrice = refPrice,
                    ChangedImportPrice = importPrice,
                    ProductId = productId
                };
            }


            private Product GetUpdateProduct(string productId, ImportData data, int addedUnitCount)
            {
                var productToUpdate = new Product
                {
                    Id = productId,
                    Brand = data.Brand,
                    Name = data.Name,
                    Pattern = data.Pattern,
                    Price = data.Price,
                    Type = data.Type,
                    Size = data.Size,
                    QuantityRemain = data.Quantity + addedUnitCount,
                    //ModifyBy = ,
                    ModifyDate = _now
                };
                return productToUpdate;
            }

            private Product GetCreateProduct(string productId, ImportData data)
            {
                var productToCreate = new Product
                {
                    Id = productId,
                    Brand = data.Brand,
                    Name = data.Name,
                    Pattern = data.Pattern,
                    Price = data.Price,
                    Type = data.Type,
                    Size = data.Size,
                    QuantityRemain = data.Quantity,
                    //CreatedBy = ,
                    CreatedDate = _now
                };
                return productToCreate;

            }


            private async Task<Product> GetProductByProps(string pattern, string type, string brand, string size)
            {
                var product = await _context.Product
                    .Include(p => p.PriceFluctuation.OrderByDescending(pf => pf.Date).FirstOrDefault())
                    .FirstOrDefaultAsync(p => p.Pattern.Equals(pattern) && p.Type.Equals(type)
                        && p.Brand.Equals(brand) && p.Size.Equals(size));
                return product;
            }


        }
    }
}
