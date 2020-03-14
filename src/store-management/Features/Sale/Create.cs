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
            public int SaleAmount { get; set; }
            public decimal SalePrice { get; set; }
        }

        public class SaleDataValidator : AbstractValidator<SaleData>
        {
            public SaleDataValidator()
            {
                RuleFor(x => x.ProductId).NotNull().NotEmpty();
                RuleFor(x => x.SaleAmount).NotNull().NotEmpty().GreaterThan(0);
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
                    List<ProductSale> saleUnits = new List<ProductSale>();
                    List<ProductImport> productImportsToUpdate = new List<ProductImport>();

                    var transactionId = Guid.NewGuid().ToString();
                    var transaction = new Transaction
                    {
                        Id = transactionId,
                        Billing = false,
                        Type = "S",
                        Date = _now
                    };

                    await _context.Transaction.AddAsync(transaction, cancellationToken);

                    // When a product is sold, 3 tables will be updated:
                    // 1. Create Product Sale record
                    // 2. Create Relationship between Product Sale and Product Import => get the oldest Product Import with remaining quantity higher than 0
                    //  2.1. Substract Product Import quantity 
                    // 3. Add product sale quantity to Product Export for billing later

                    foreach (var item in request.SalesData)
                    {
                        var productToSell = await _context.Product
                            .Include(p => p.ProductImport)
                            .FirstOrDefaultAsync(p => p.Id.Equals(item.ProductId));
                            
                        if (productToSell != null)
                        {
                            if (productToSell.TotalQuantity < item.SaleAmount) // if sale amount is greater than available quantity a product has
                                throw new RestException(HttpStatusCode.Conflict, new { Error = "Số lượng bán nhiều hơn số lượng sản phẩm có trong kho" });
                            productToSell.NoBillRemainQuantity += item.SaleAmount;
                            _context.Product.Update(productToSell);
                            // when selling a product, the oldest Product Import with remaining quantity higher than 0 will be substracted
                            var productImports = productToSell.ProductImport.Where(pi => pi.RemainQuantity > 0).OrderBy(pi => pi.Date).ToList();
                            if (productImports.Count != 0)
                            {
                                // with each productToSell - create a record in ProductSale table
                                string pdSaleId = Guid.NewGuid().ToString();

                                saleUnits.Add(new ProductSale
                                {
                                    Id = pdSaleId,
                                    ProductId = productToSell.Id,
                                    SaleAmount = item.SaleAmount,
                                    SalePrice = item.SalePrice,
                                    TransactionId = transactionId
                                });
                                var saleAmount = item.SaleAmount;
                                for (int i = 0; i < productImports.Count; i++)
                                {
                                    // if remaining quantity is greater than sale amount => update remain quantity by current remain quantity substract sale amount
                                    if (productImports[i].RemainQuantity > saleAmount)
                                    {
                                        await _context.SaleImportReport.AddAsync(new SaleImportReport
                                        {
                                            ProductImportId = productImports[i].Id,
                                            ProductSaleId = pdSaleId,
                                            Quantity = saleAmount
                                        });
                                        productImports[i].RemainQuantity -= saleAmount;
                                        _context.ProductImport.UpdateRange(productImports);

                                        break;
                                    }
                                    // if remaining quantity is lower than sale amount => remain quantity is set to 0 and continue loop
                                    else
                                    {
                                        saleAmount -= productImports[i].RemainQuantity.Value;
                                        await _context.SaleImportReport.AddAsync(new SaleImportReport
                                        {
                                            ProductImportId = productImports[i].Id,
                                            ProductSaleId = pdSaleId,
                                            Quantity = productImports[i].RemainQuantity.Value
                                        });
                                        productImports[i].RemainQuantity = 0;
                                        _context.ProductImport.UpdateRange(productImports);
                                    }

                                }

                            }
                            else // if a product don't have any product import which has quantity left
                            {
                                throw new RestException(HttpStatusCode.Conflict, new { Error = "Số lượng sản phẩm không đủ" });
                            }
                            // Product Export

                        } 
                        else
                            throw new RestException(HttpStatusCode.Conflict, new { Error = "Sản phẩm được bán không tồn tại" });
                    }
                    
                    await _context.ProductSale.AddRangeAsync(saleUnits, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    return new SaleEnvelope(saleUnits);
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { Error = "Giá trị nhập vào không hợp lệ" });
            }
        }
    }
}
