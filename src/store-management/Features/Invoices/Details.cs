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
    public class Details
    {
        public class Query : IRequest<InvoiceEnvelope>
        {
            public Query(string invoiceNo)
            {
                InvoiceNo = invoiceNo;
            }
            public string InvoiceNo { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.InvoiceNo).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, InvoiceEnvelope>
        {
            private readonly StoreContext _context;

            public QueryHandler(StoreContext context)
            {
                _context = context;
            }

            public async Task<InvoiceEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var invoice = await _context.Invoice
                    .Include(i => i.InvoiceLine)
                    .FirstOrDefaultAsync(i => i.InvoiceNo.Equals(request.InvoiceNo));
                if (invoice == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Error = "Không tồn tại mã hóa đơn trong hệ thống" });
                return new InvoiceEnvelope(invoice);
            }
        }


    }


}
