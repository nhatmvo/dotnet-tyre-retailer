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

namespace store_management.Features.Invoices
{
    public class Create
    {
        public class InvoiceData
        {
            public string ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal? SoldPrice { get; set; }
        }

        public class InvoiceDataValidator : AbstractValidator<InvoiceData>
        {
            public InvoiceDataValidator()
            {
                RuleFor(x => x.ProductId).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<InvoiceEnvelope>
        {
            public List<InvoiceData> InvoiceLinesData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.InvoiceLinesData).NotNull().NotEmpty()
                    .SetValidator(new InvoiceDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, InvoiceEnvelope>
        {
            private readonly StoreContext _context;
            
            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<InvoiceEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var lines = new List<InvoiceLine>();

                var invoice = new Invoice()
                {
                    Id = Guid.NewGuid().ToString(),
                    ExportDate = DateTime.Now,
                    InvoiceNo = (new Random()).Next(10000000, 99999999),
                };
                

                foreach (var item in request.InvoiceLinesData)
                {

                    var notBillingProduct = await _context.ProductExport
                        .Include(pe => pe.Product)
                        .FirstOrDefaultAsync(pe => pe.ProductId.Equals(item.ProductId));

                    if (notBillingProduct == null) 
                        throw new RestException(HttpStatusCode.BadRequest, new { });
                    if (notBillingProduct.NotBillRemainQuantity < item.Quantity)
                        throw new RestException(HttpStatusCode.BadRequest, new { });

                    var exportPrice = item.SoldPrice ?? notBillingProduct.Product.RefPrice;
                    var invoiceLine = new InvoiceLine()
                    {
                        ExportPrice = exportPrice,
                        Id = Guid.NewGuid().ToString(),
                        InvoiceId = invoice.Id,
                        Quantity = item.Quantity,
                        Total = exportPrice * item.Quantity,
                        ProductId = item.ProductId
                    };
                    lines.Add(invoiceLine);
                    
                }

                invoice.Total = lines.Sum(l => l.Total);
                await _context.Invoice.AddAsync(invoice);
                await _context.InvoiceLine.AddRangeAsync(lines);

                await _context.SaveChangesAsync();

                return new InvoiceEnvelope(invoice);
            }
        }
    }
}
