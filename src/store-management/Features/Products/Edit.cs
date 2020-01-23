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
using System.Text.Json.Serialization;

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
            public string Description { get; set; }
        }

        public class Command : IRequest<ProductEnvelope>
        {
            public ProductData ProductData { get; set; }
            [JsonIgnore]
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
                    .FirstOrDefaultAsync(p => p.Id.Equals(request.Id));
                if (productToUpdate == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });

                // check properties of product to insert
                productToUpdate.Name = string.IsNullOrEmpty(request.ProductData.Name) ? productToUpdate.Name : request.ProductData.Name;
                productToUpdate.Type = string.IsNullOrEmpty(request.ProductData.Type) ? productToUpdate.Type : request.ProductData.Type;
                productToUpdate.Size = string.IsNullOrEmpty(request.ProductData.Size) ? productToUpdate.Size : request.ProductData.Size;
                productToUpdate.Brand = string.IsNullOrEmpty(request.ProductData.Brand) ? productToUpdate.Brand : request.ProductData.Brand;
                productToUpdate.Pattern = string.IsNullOrEmpty(request.ProductData.Pattern) ? productToUpdate.Pattern : request.ProductData.Pattern;
                productToUpdate.ImagePath = string.IsNullOrEmpty(request.ProductData.ImagePath) ? productToUpdate.ImagePath : request.ProductData.ImagePath;
                productToUpdate.Description = string.IsNullOrEmpty(request.ProductData.Description) ? productToUpdate.Description : request.ProductData.Description;


                // Update last modify by and last modify date
                productToUpdate.ModifiedDate = DateTime.Now;

                _context.Product.Update(productToUpdate);

                await _context.SaveChangesAsync();

                return new ProductEnvelope(productToUpdate);
                
            }
        }
    }
}
