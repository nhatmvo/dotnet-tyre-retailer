using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;

namespace store_management.Features.Products
{
    public class Edit
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

        public class Command : IRequest<ProductEnvelope>
        {
            public ProductData ProductData { get; set; }
            public string Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, ProductEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;


            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ProductEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var productToUpdate = await _context.Product
                    .FirstOrDefaultAsync(p => (new Guid(p.Id)).ToString() == request.Id);
                if (productToUpdate == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });

                if (request.ProductData.Price != 0 || request.ProductData.ImportPrice != 0)
                {
                    var pHistory  = _context.PriceFluctuation.FromSqlRaw($"SELECT IMPORT_PRICE FROM PRICE_FLUCTUATION " +
                        $"WHERE PRODUCT_ID = '{request.Id}' ORDER BY DATE DESC").FirstOrDefault();
                    var priceFluctuation = new PriceFluctuation
                    {
                        Id = Guid.NewGuid().ToByteArray(),
                        ChangedImportPrice = request.ProductData.ImportPrice != 0 ? request.ProductData.ImportPrice : pHistory.ChangedImportPrice,
                        CurrentImportPrice = pHistory.ChangedImportPrice,
                        ChangedPrice = request.ProductData.Price != 0 ? request.ProductData.Price : productToUpdate.Price,
                        CurrentPrice = pHistory.ChangedPrice,
                        Date = DateTime.Now,
                        ProductId = (new Guid(request.Id)).ToByteArray()
                        
                    };
                    await _context.PriceFluctuation.AddAsync(priceFluctuation);
                }


                // check properties of product to insert
                productToUpdate.Name = request.ProductData.Name ?? productToUpdate.Name;
                productToUpdate.Type = request.ProductData.Type ?? productToUpdate.Type;
                productToUpdate.Size = request.ProductData.Size ?? productToUpdate.Size;
                productToUpdate.Brand = request.ProductData.Brand ?? productToUpdate.Brand;
                productToUpdate.Pattern = request.ProductData.Pattern ?? productToUpdate.Pattern;
                productToUpdate.ImagePath = request.ProductData.ImagePath ?? productToUpdate.ImagePath;
                productToUpdate.Price = request.ProductData.Price != 0 ? request.ProductData.Price : productToUpdate.Price;
                productToUpdate.QuantityRemain = request.ProductData.Quantity != 0 ? request.ProductData.Quantity : productToUpdate.QuantityRemain;
                productToUpdate.Description = request.ProductData.Description ?? productToUpdate.Description;

                // Update last modify by and last modify date
                productToUpdate.ModifyDate = DateTime.Now;

                _context.Product.Update(productToUpdate);

                //var reportHistory = new TxReport
                //{
                //    Id = Guid.NewGuid().ToByteArray(),
                //    Action = ActionConstants.EDIT_PRODUCT,
                //    CreateTime = DateTime.Now,
                //    ProductId = new Guid(request.Id).ToByteArray(),
                //    QuantityUpdate = request.ProductData.Quantity
                //};

                await _context.SaveChangesAsync();

                return new ProductEnvelope(productToUpdate);
                
            }
        }
    }
}
