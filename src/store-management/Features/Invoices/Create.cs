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
            public string ExportUnitId { get; set; }
            public decimal? ExportPrice { get; set; }
        }

        public class InvoiceDataValidator : AbstractValidator<InvoiceData>
        {
            public InvoiceDataValidator()
            {
                RuleFor(x => x.ExportUnitId).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<InvoiceEnvelope>
        {
            public List<InvoiceData> InvoiceData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.InvoiceData).NotNull().NotEmpty();
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

                foreach (var item in request.InvoiceData)
                {
                    var currentUnit = await _context.SoldUnit.FirstOrDefaultAsync(su => su.Id.Equals(item.ExportUnitId));
                    var product = (from p in _context.Product
                                   join pf in _context.PriceFluctuation on p.Id equals pf.ProductId
                                   join s in _context.SoldUnit on pf.Id equals s.PriceFluctuationId
                                   where s.Id == item.ExportUnitId
                                   select new
                                   {
                                       ProductName = p.Name,
                                       ProductType = p.Type
                                   }).FirstOrDefault();

                    var exportPrice = item.ExportPrice ?? currentUnit.SalePrice;
                    var invoiceLine = new InvoiceLine()
                    {
                        ExportPrice = exportPrice,
                        ProductName = product.ProductName,
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
