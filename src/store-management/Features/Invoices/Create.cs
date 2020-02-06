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
            public int ExportAmount { get; set; }
            public decimal? ExportPrice { get; set; }

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
            public string CustomerTaxCode { get; set; }
            public string CustomerBankAccountNo { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAddress { get; set; }
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

                var customer = new Customer()
                {
                    Address = request.CustomerAddress,
                    BankAccountNumber = request.CustomerBankAccountNo,
                    FullName = request.CustomerName,
                    Id = Guid.NewGuid().ToString(),
                    TaxCode = request.CustomerTaxCode
                };



                var invoice = new Invoice()
                {
                    Id = Guid.NewGuid().ToString(),
                    ExportDate = DateTime.Now,
                    InvoiceNo = (new Random()).Next(10000000, 99999999),
                    CustomerId = customer.Id
                };
                

                foreach (var item in request.InvoiceLinesData)
                {

                    var notBillingProduct = await _context.ProductExport
                        .Include(pe => pe.Product)
                        .FirstOrDefaultAsync(pe => pe.ProductId.Equals(item.ProductId));

                    if (notBillingProduct == null) 
                        throw new RestException(HttpStatusCode.BadRequest, new { });
                    if (notBillingProduct.NoBillRemainQuantity < item.ExportAmount)
                        throw new RestException(HttpStatusCode.BadRequest, new { });

                    var exportPrice = item.ExportPrice ?? notBillingProduct.Product.RefPrice;
                    var invoiceLine = new InvoiceLine()
                    {
                        ExportPrice = exportPrice,
                        Id = Guid.NewGuid().ToString(),
                        InvoiceId = invoice.Id,
                        ExportAmount = item.ExportAmount,
                        Total = exportPrice * item.ExportAmount,
                        ProductId = item.ProductId
                    };
                    lines.Add(invoiceLine);
                    
                }

                invoice.Total = lines.Sum(l => l.Total);
                await _context.Customer.AddAsync(customer);
                await _context.Invoice.AddAsync(invoice);
                await _context.InvoiceLine.AddRangeAsync(lines);

                await _context.SaveChangesAsync();

                return new InvoiceEnvelope(invoice);
            }
        }
    }
}
