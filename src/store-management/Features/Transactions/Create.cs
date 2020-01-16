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
            public TransactionData TransactionData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TransactionData).NotNull().SetValidator(new TransactionValidation());
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
                var productToSell = await _context.Product.FromSqlRaw($"SELECT TYPE FROM PRODUCT WHERE ID = {request.TransactionData.ProductId}").FirstOrDefaultAsync();
                List<ExportUnit> exportUnits = new List<ExportUnit>();
                if (productToSell != null)
                {
                    // Get latest price fluctuation from a Product
                    var productPrice = await _context.PriceFluctuation
                        .FromSqlRaw($"SELECT ID, CHANGED_PRICE FROM PRICE_FLUCTUATION WHERE PRODUCT_ID = {request.TransactionData.ProductId} ORDER BY DATE DESC")
                        .FirstOrDefaultAsync();
                    // Warranty Code only have effect when product type is TIRE
                    if (productToSell.Type.Equals(ProductTypes.TIRE))
                    {
                        for (int i = 0; i < request.TransactionData.Quantity; i++)
                        {
                            // Export price will be update when export invoice
                            exportUnits.Add(new ExportUnit
                            {
                                Id = Guid.NewGuid().ToByteArray(),
                                WarrantyCode = (new Random()).Next(10000000, 99999999).ToString(),
                                Type = ProductTypes.TIRE,
                                Billing = false,
                                Quantity = 1,
                                ExportPrice = productPrice.ChangedPrice,
                                ExportDatetime = DateTime.Now,
                                PriceFluctuationId = productPrice.Id
                            });
                        }
                    }
                    else // If the type is different than TIRE => insert with multiple quantity without warranty code
                    {
                        exportUnits.Add(new ExportUnit
                        {
                            Id = Guid.NewGuid().ToByteArray(),
                            WarrantyCode = string.Empty,
                            Type = productToSell.Type,
                            Billing = false,
                            Quantity = request.TransactionData.Quantity,
                            ExportDatetime = DateTime.Now,
                            ExportPrice = productPrice.ChangedPrice,
                            PriceFluctuationId = productPrice.Id
                        });
                    }
                }
                else throw new Exception();

                await _context.ExportUnit.AddRangeAsync(exportUnits);
                await _context.SaveChangesAsync();

                return new TransactionsEnvelope(exportUnits);
            }
        }




        
    }
}
