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
            private readonly DateTime _now;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _now = DateTime.Now;
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ProductEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var product = await _context.Product.FirstOrDefaultAsync(p => p.Type.Equals(request.ProductData.Type)
                            && p.Name.Equals(request.ProductData.Name) && p.Brand.Equals(request.ProductData.Brand) && p.Pattern.Equals(request.ProductData.Pattern));
                    if (product != null)
                        throw new RestException(HttpStatusCode.BadRequest, new { });
                    var productId = Guid.NewGuid().ToString();
                    var productToCreate = new Product
                    {
                        Id = productId,
                        Name = request.ProductData.Name,
                        Type = request.ProductData.Type,
                        Brand = request.ProductData.Brand,
                        Pattern = request.ProductData.Pattern,
                        Description = request.ProductData.Description,
                        CreatedDate = _now
                        // Add created person
                    };

                    await _context.Product.AddAsync(productToCreate);
                    await _context.SaveChangesAsync();

                    return new ProductEnvelope(productToCreate);
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });


            }
        }
    }
}
