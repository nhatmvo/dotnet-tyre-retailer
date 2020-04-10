using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            private readonly CustomLogger _logger;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _now = DateTime.Now;
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _logger = new CustomLogger();
            }

            public async Task<ProductEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var product = await _context.Product.FirstOrDefaultAsync(p => p.Type.Equals(request.ProductData.Type)
                            && p.Name.Equals(request.ProductData.Name) && p.Brand.Equals(request.ProductData.Brand) && p.Pattern.Equals(request.ProductData.Pattern));
                    if (product != null)
                        throw new RestException(HttpStatusCode.BadRequest, new { Error = "Sản phẩm đã tồn tại trong hệ thống" });
                    var productId = Guid.NewGuid().ToString();

                    var username = _currentUserAccessor.GetCurrentUsername();
                    var productToCreate = new Product
                    {
                        Id = productId,
                        Name = request.ProductData.Name,
                        Type = request.ProductData.Type,
                        Brand = request.ProductData.Brand,
                        Pattern = request.ProductData.Pattern,
                        Description = request.ProductData.Description,
                        CreatedDate = _now,
                        CreatedBy = username
                    };
                    // Add logg
                    _logger.AddLog(_context, username, username + " thêm mới thông tin sản phẩm " + productToCreate.Name + " vào ngày " + _now.ToString(CultureInfo.CurrentCulture), "Tạo mới");

                    await _context.Product.AddAsync(productToCreate, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    return new ProductEnvelope(productToCreate);
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { Error = "Dữ liệu đầu vào không hợp lệ" });


            }
        }
    }
}
