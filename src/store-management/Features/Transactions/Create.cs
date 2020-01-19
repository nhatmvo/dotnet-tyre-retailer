using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Transactions
{
    public class Create
    {
        public class TransactionData
        {
            public string ProductId { get; set; }
            public decimal SalePrice { get; set; }
            public int Quantity { get; set; }
            public string Description { get; set; }
        }

        public class TransactionValidation : AbstractValidator<TransactionData>
        {
            public TransactionValidation()
            {
                RuleFor(x => x.Quantity).NotEmpty().NotNull().GreaterThan(0);
            }
        }

        public class Command : IRequest<TransactionsEnvelope>
        {
            public List<TransactionData> TransactionData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TransactionData).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, TransactionsEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;


            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<TransactionsEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                List<SoldUnit> soldUnits = new List<SoldUnit>();
                foreach (var item in request.TransactionData)
                {
                    var productToSell = await _context.Product.Where(p => p.Id.Equals(item.ProductId)).FirstOrDefaultAsync();
                    if (productToSell != null)
                    {
                        
                        productToSell.QuantityRemain = productToSell.QuantityRemain - item.Quantity;
                        _context.Product.Update(productToSell);

                        // Get latest price fluctuation from a Product
                        var productPrice = await _context.PriceFluctuation
                            .Where(pf => pf.ProductId.Equals(item.ProductId))
                            .OrderByDescending(p => p.Date)
                            .FirstOrDefaultAsync();

                        // Warranty Code only have effect when product type is TIRE
                        if (productToSell.Type.Equals(ProductTypes.TIRE))
                        {
                            for (int i = 0; i < item.Quantity; i++)
                            {
                                // Export price will be update when export invoice
                                soldUnits.Add(new SoldUnit
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    WarrantyCode = (new Random()).Next(10000000, 99999999).ToString(),
                                    Type = ProductTypes.TIRE,
                                    Billing = false,
                                    Quantity = 1,
                                    ReferPrice = productPrice.ChangedPrice,
                                    SalePrice = item.SalePrice,
                                    //ExportDatetime = DateTime.Now,
                                    PriceFluctuationId = productPrice.Id
                                });
                            }
                        }
                        else // If the type is different than TIRE => insert with multiple quantity without warranty code
                        {
                            soldUnits.Add(new SoldUnit
                            {
                                Id = Guid.NewGuid().ToString(),
                                WarrantyCode = string.Empty,
                                Type = productToSell.Type,
                                Billing = false,
                                Quantity = item.Quantity,
                                //ExportDatetime = DateTime.Now,
                                ReferPrice = productPrice.ChangedPrice,
                                SalePrice = item.SalePrice,
                                PriceFluctuationId = productPrice.Id
                            });
                        }
                    }
                    else throw new Exception();
                    await _context.SoldUnit.AddRangeAsync(soldUnits);
                }
                
                await _context.SaveChangesAsync();

                return new TransactionsEnvelope(soldUnits);
            }
        }




        
    }
}
