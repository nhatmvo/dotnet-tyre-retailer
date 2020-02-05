    using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Features.Imports;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static store_management.Features.Imports.Create;

namespace store_management.Features.Imports
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
            public decimal ImportPrice { get; set; }
            public int ImportAmount { get; set; }
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
                RuleFor(x => x.ImportPrice).NotEmpty().NotNull().GreaterThan(0);
                RuleFor(x => x.ImportAmount).NotEmpty().NotNull().GreaterThan(0);
            }
        }

        public class Command : IRequest<ImportEnvelope>
        {
            public List<ImportData> ImportsData { get; set; }
            public string Note { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.ImportsData).SetValidator(new ImportDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, ImportEnvelope>
        {

            private readonly StoreContext _context;
            private readonly DateTime _now;

            public Handler(StoreContext context)
            {
                _context = context;
                _now = DateTime.Now;
            }

            public async Task<ImportEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    request.ImportsData = HandleDuplicateObjRequest(request.ImportsData);

                    string transactionId = Guid.NewGuid().ToString();

                    var productsInsert = new List<Product>();
                    var productsUpdate = new List<Product>();
                    var productImports = new List<ProductImport>();

                    // Create a transaction 
                    var transaction = new Transaction
                    {
                        Id = transactionId,
                        Type = TransactionType.IMPORT,
                        Billing = false,
                        Date = _now,
                        Note = request.Note
                    };
                    await _context.Transaction.AddAsync(transaction);

                    foreach (var item in request.ImportsData)
                    {
                        // for each product created or updated, the following tables is also inserted
                        //  1. Product - contains information about a Product
                        //  2. PriceFluctuation - updated with lastest price of a product
                        //  3. IeReport - using for export & import information when create report
                        var product = await GetProductByProps(item.Pattern, item.Type, item.Brand, item.Size);
                        string productId;
                        if (product == null)
                        {
                            productId = Guid.NewGuid().ToString();
                            productsInsert.Add(GetCreateProduct(productId, item));
                        }
                        else
                        {
                            productId = product.Id;
                            productsUpdate.Add(GetUpdateProduct(product, item, product.TotalQuantity));
                        }
                        var productImport = new ProductImport
                        {
                            Id = Guid.NewGuid().ToString(),
                            ImportPrice = item.ImportPrice,
                            ProductId = productId,
                            Date = DateTime.Now,
                            ImportAmount = item.ImportAmount,
                            RemainQuantity = item.ImportAmount,
                            TransactionId = transactionId,
                            ProductTotalQuantity = product != null ? product.TotalQuantity + item.ImportAmount : item.ImportAmount
                        };
                        productImports.Add(productImport);


                    }
                    await _context.Product.AddRangeAsync(productsInsert);
                    _context.Product.UpdateRange(productsUpdate);

                    await _context.ProductImport.AddRangeAsync(productImports);

                    await _context.SaveChangesAsync();

                    return new ImportEnvelope
                    {
                        ProductImports = productImports
                    };
                } else
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { });
                }
            }


            private Product GetUpdateProduct(Product productToUpdate, ImportData data, int addedUnitCount)
            {
                productToUpdate.RefPrice = data.ImportPrice;
                productToUpdate.ModifiedDate = _now;
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
                    Type = data.Type,
                    Size = data.Size,
                    RefPrice = data.ImportPrice,
                    //CreatedBy = ,
                    CreatedDate = _now
                };
                return productToCreate;

            }


            private async Task<Product> GetProductByProps(string pattern, string type, string brand, string size)
            {
                var product = await _context.Product.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Pattern.Equals(pattern) && p.Type.Equals(type)
                        && p.Brand.Equals(brand) && p.Size.Equals(size));
                return product;
            }

            private List<ImportData> HandleDuplicateObjRequest(List<ImportData> importsData)
            {
                var handledResult = new List<ImportData>();
                foreach (var item in importsData)
                {
                    var existedItem = handledResult.Where(a => a.Brand.Equals(item.Brand) && a.Type.Equals(item.Type) && a.Pattern.Equals(item.Pattern) && a.Size.Equals(item.Size)).FirstOrDefault();
                    if (existedItem != null)
                    {
                        if (existedItem.ImportPrice != item.ImportPrice) throw new RestException(HttpStatusCode.BadRequest, new { });
                        existedItem.ImportAmount += item.ImportAmount;
                    } 
                    else
                    {
                        handledResult.Add(item);
                    }
                }
                return handledResult;
            }


        }
    }
}
