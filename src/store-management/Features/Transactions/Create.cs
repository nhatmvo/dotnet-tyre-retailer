using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
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

        public class Command : IRequest<TransactionEnvelope>
        {
            public string CustomerId { get; set; }
            public InvoiceTypes InvoiceStatus { get; set; }
            public string Detail { get; set; }
            // according with InvoiceLine value
            public List<TransactionData> TransactionParts { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {

        }

        public class Handler : IRequestHandler<Command, TransactionEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;


            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<TransactionEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var invoiceId = Guid.NewGuid().ToByteArray();

                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => request.CustomerId.Equals((new Guid(c.Id)).ToString()));
                if (customer == null)
                {
                    request.Detail = "Không xuất hóa đơn cho khách hàng";
                }

                var invoice = new Invoice()
                {
                    Id = invoiceId,
                    ExportDate = DateTime.Now,
                    Detail = request.Detail,
                    CustomerId = customer == null ? customer.Id : null,
                };

                await _context.Invoice.AddAsync(invoice);
                await _context.SaveChangesAsync();

                // Create each line in a transaction (invoice)
                List<InvoiceLine> lines = new List<InvoiceLine>();
                foreach (var part in request.TransactionParts)
                {
                    var productLine = await _context.Product
                        .FirstOrDefaultAsync(p => part.ProductId.Equals((new Guid(p.Id)).ToString()), cancellationToken);
                    lines.Add(new InvoiceLine()
                    {
                        Id = Guid.NewGuid().ToByteArray(),
                        ProductId = (new Guid(part.ProductId)).ToByteArray(),
                        Quantity = part.Quantity,
                        Total = part.Quantity * productLine.Price,
                        Description = part.Description,
                        InvoiceId = invoiceId
                    });
                   
                }
                invoice.InvoiceLine = lines;
                await _context.InvoiceLine.AddRangeAsync(lines);
                await _context.SaveChangesAsync();

                return new TransactionEnvelope(invoice);
            }
        }




        
    }
}
