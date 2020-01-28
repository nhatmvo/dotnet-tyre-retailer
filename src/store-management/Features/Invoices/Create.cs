using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
    public class Create
    {
        public class InvoiceData
        {
            public string SoldUnitId { get; set; }
            public decimal? SoldPrice { get; set; }
        }

        public class InvoiceDataValidator : AbstractValidator<InvoiceData>
        {
            public InvoiceDataValidator()
            {
                RuleFor(x => x.SoldUnitId).NotNull().NotEmpty();
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
                await _context.Invoice.AddAsync(invoice);

                foreach (var item in request.InvoiceLinesData)
                {
                    var currentUnit = await _context.ProductSale.Include(ps => ps.Product)
                        .FirstOrDefaultAsync(su => su.Id.Equals(item.SoldUnitId));

                    var exportPrice = item.SoldPrice ?? currentUnit.SalePrice;
                    var invoiceLine = new InvoiceLine()
                    {
                        ExportPrice = exportPrice,
                        Id = Guid.NewGuid().ToString(),
                        InvoiceId = invoice.Id,
                        Quantity = currentUnit.Quantity,
                        Total = exportPrice * currentUnit.Quantity
                    };
                    lines.Add(invoiceLine);
                    await _context.InvoiceLine.AddAsync(invoiceLine);
                }

                invoice.Total = lines.Sum(l => l.Total);
                _context.Invoice.Update(invoice);

                await _context.SaveChangesAsync();

                return new InvoiceEnvelope(invoice);
            }
        }

    }
}
