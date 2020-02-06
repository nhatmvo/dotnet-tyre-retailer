using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Reports
{
    public class ProdDetails
    {
        public class ProductReportFilter
        {
            public string ProductId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class ProductReportFilterValidator : AbstractValidator<ProductReportFilter>
        {
            public ProductReportFilterValidator()
            {
                RuleFor(x => x.ProductId).NotNull().NotEmpty();
            }
        }

        public class Query : IRequest<ReportsEnvelope>
        {
            public Query() { }

            public Query(ProductReportFilter filter)
            {
                Filter = filter;
            }

            public ProductReportFilter Filter { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query> 
        { 
            public QueryValidator()
            {
                RuleFor(x => x.Filter).SetValidator(new ProductReportFilterValidator());
            }    
        }

        public class Handler : IRequestHandler<Query, ReportsEnvelope>
        {
            private readonly StoreContext _context;
            private readonly DateTime _now;

            public Handler(StoreContext context)
            {
                _context = context;
                _now = DateTime.Now;
            }

            public async Task<ReportsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var validator = (new QueryValidator()).Validate(request);
                if (validator.IsValid)
                {
                    var queryable = _context.Product.Where(p => p.Id.Equals(request.Filter.ProductId))
                        .Include(p => p.ProductImport)
                        .Include(p => p.ProductSale)
                        .Include(p => p.InvoiceLine).AsQueryable();

                    if (request.Filter.StartDate != null)
                    {
                        queryable = queryable.Where(p => p.ProductImport.Any(pi => pi.Date > request.Filter.StartDate) 
                            && p.ProductSale.Any(ps => ps.Transaction.Date > request.Filter.StartDate) 
                            && p.InvoiceLine.Any(il => il.Invoice.ExportDate > request.Filter.StartDate));
                    }
                    if (request.Filter.EndDate != null)
                    {
                        queryable = queryable.Where(p => p.ProductImport.Any(pi => pi.Date < request.Filter.EndDate)
                            && p.ProductSale.Any(ps => ps.Transaction.Date < request.Filter.EndDate)
                            && p.InvoiceLine.Any(il => il.Invoice.ExportDate < request.Filter.EndDate));
                    }
                    var productReports = await queryable.FirstOrDefaultAsync(cancellationToken);
                    var importsEnvelope = productReports.ProductImport.Select(p => new ReportEnvelope 
                    { 
                        Action = TransactionType.IMPORT,
                        Price = p.ImportPrice,
                        Quantity = p.ImportAmount.Value,
                        Timestamp = p.Date
                    });

                    var salesEnvelope = productReports.ProductSale.Select(p => new ReportEnvelope
                    {
                        Action = TransactionType.SOLD,
                        Price = p.SalePrice.Value,
                        Quantity = p.SaleAmount.Value,
                        Timestamp = p.Transaction.Date
                    });

                    var exportEnvelope = productReports.InvoiceLine.Select(p => new ReportEnvelope
                    {
                        Action = TransactionType.EXPORT,
                        Price = p.ExportPrice.Value,
                        Quantity = p.ExportAmount.Value,
                        Timestamp = p.Invoice.ExportDate
                    });
                    return new ReportsEnvelope(importsEnvelope.Concat(salesEnvelope).Concat(exportEnvelope).ToList());
                }
                else
                    throw new RestException(HttpStatusCode.BadRequest, new { });
            }
        }


    }
}
