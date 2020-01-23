using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Sale
{
    public class Create
    {
        public class SaleData
        {
            public string ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal SalePrice { get; set; }
        }

        public class SaleDataValidator : AbstractValidator<SaleData>
        {
            public SaleDataValidator()
            {
                RuleFor(x => x.ProductId).NotNull().NotEmpty();
                RuleFor(x => x.Quantity).NotNull().NotEmpty().GreaterThan(0);
                RuleFor(x => x.SalePrice).NotNull().NotEmpty().GreaterThan(0);
            }
        }

        public class Command : IRequest<SaleEnvelope>
        {
            public List<SaleData> SalesData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.SalesData).SetValidator(new SaleDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, SaleEnvelope>
        {
            private readonly StoreContext _context;
            private readonly DateTime _now;

            public Handler(StoreContext context)
            {
                _context = context;
                _now = DateTime.Now;
            }

            public async Task<SaleEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var validator = (new CommandValidator()).Validate(request);
                if (validator.IsValid)
                {
                    List<SaleUnit> saleUnits = new List<SaleUnit>();
                    List<Product> updateProducts = new List<Product>();

                    var transactionId = Guid.NewGuid().ToString();
                    var transaction = new Transaction
                    {
                        Id = transactionId,
                        Billing = false,
                        Type = "S",
                        Date = _now
                    };

                    await _context.AddAsync(transaction, cancellationToken);

                    foreach (var item in request.SalesData)
                    {
                        var productToSell = await _context.Product.FirstOrDefaultAsync(p => p.Id.Equals(item.ProductId));
                        if (productToSell != null)
                        {
                            var productPrice = await _context.PriceFluctuation
                                .AsNoTracking()
                                .OrderByDescending(pf => pf.Date)
                                .FirstOrDefaultAsync(pf => pf.ProductId.Equals(item.ProductId), cancellationToken);
                            saleUnits.Add(new SaleUnit
                            {
                                Id = Guid.NewGuid().ToString(),
                                Billing = false,
                                PriceFluctuationId = productPrice.Id,
                                Quantity = item.Quantity,
                                SalePrice = item.SalePrice,
                                ReferPrice = productPrice.ChangedPrice,
                                TransactionId = transactionId
                            });

                            productToSell.QuantityRemain -= item.Quantity;
                            if (productToSell.QuantityRemain < 0) throw new RestException(HttpStatusCode.BadRequest, new { });
                            updateProducts.Add(productToSell);

                        } 
                        else
                            throw new RestException(HttpStatusCode.Conflict, new { });
                    }
                    _context.Product.UpdateRange(updateProducts);
                    await _context.SaleUnit.AddRangeAsync(saleUnits, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);
                    return new SaleEnvelope(saleUnits);
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });
            }
        }
    }
}
