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
                RuleFor(x => x.Quantity).NotEmpty().NotNull().GreaterThan(0);
            }
        }

        public class Command : IRequest<ProductEnvelope>
        {
            public ProductData ProductData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProductData).SetValidator(new ProductDataValidator());
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
                var product = await _context.Product.FirstOrDefaultAsync(p => p.Name == request.ProductData.Name);
                if (product != null)
                    throw new RestException(HttpStatusCode.BadRequest, new { Product = Constants.EXISTED });
                var productToCreate = new Product()
                {
                    Id = Guid.NewGuid().ToByteArray(),
                    Name = request.ProductData.Name,
                    Type = request.ProductData.Type,
                    Brand = request.ProductData.Brand,
                    Pattern = request.ProductData.Pattern,
                    Price = request.ProductData.Price,
                    Quality = request.ProductData.Quantity,
                    Description = request.ProductData.Description,
                    CreatedDate = DateTime.Now
                    // Add created person
                };

                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();

                return new ProductEnvelope(product);
            }
        }
    }
}
