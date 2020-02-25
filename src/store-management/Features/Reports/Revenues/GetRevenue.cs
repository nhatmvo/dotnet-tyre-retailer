using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Revenues
{
    public class GetRevenue
    {
        public class Query : IRequest<RevenuesEnvelope>
        {
            public Query() { }

            public Query(RevenueParamsFilter filter) 
            {
                Filter = filter;
            }

            public RevenueParamsFilter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, RevenuesEnvelope>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<RevenuesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.Filter != null)
                {
                    DateTime now = DateTime.Now;
                    // if none of Datetime Filter has value => StartDate and EndDate will be set in a current month
                    if (!(request.Filter.StartDate.HasValue && request.Filter.EndDate.HasValue))
                    {
                        request.Filter.StartDate = (new DateTime(now.Year, now.Month, 1)).AddHours(0).AddMinutes(0).AddSeconds(0);
                        request.Filter.EndDate = request.Filter.StartDate.Value.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    } else if (!request.Filter.StartDate.HasValue)
                        request.Filter.StartDate = (new DateTime(now.Year, now.Month, 1)).AddHours(0).AddMinutes(0).AddSeconds(0);
                    else if (!request.Filter.EndDate.HasValue)
                        request.Filter.EndDate = now;

                    var productInfoByDate = _context.Transaction.Include(p => p.ProductImport).Include(p => p.ProductSale);

                    var revenueReport = new RevenuesEnvelope();
                    var revenueByDays = new List<Dictionary<DateTime, RevenueByDay>>();

                    foreach (DateTime day in EachDay(request.Filter.StartDate.Value, request.Filter.EndDate.Value))
                    {
                        var firstDayMoment = day.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                        var lastDayMoment = day.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        var infoByDay = await productInfoByDate.Where(pbd => pbd.Date >= firstDayMoment && pbd.Date <= lastDayMoment).ToListAsync();
                        if (infoByDay.Count == 0) {
                            revenueByDays.Add(new Dictionary<DateTime, RevenueByDay>
                            {
                                { day.Date, new RevenueByDay { 
                                        SoldAmountByDay = 0,
                                        RevenuesByDay = 0,
                                        TotalImportPriceByDay = 0
                                    } 
                                }
                            });
                            continue;
                        }
                        // Sum of sale in day
                        var totalSoldByDay = infoByDay.Sum(i => i.ProductSale.Sum(ps => ps.SaleAmount * ps.SalePrice));
                        // Sum of paid for import in day
                        var totalPaidByDay = infoByDay.Sum(i => i.ProductImport.Sum(pi => pi.ImportAmount * pi.ImportPrice));
                        // Total Revenue by Day
                        var revenueByDay = totalSoldByDay - totalPaidByDay;
                        // Sum of product Sale by Day
                        var quantitySoldByDay = infoByDay.Sum(i => i.ProductSale.Sum(ps => ps.SaleAmount));
                        
                        revenueByDays.Add(new Dictionary<DateTime, RevenueByDay> {
                            { day.Date, new RevenueByDay {
                                 SoldAmountByDay = quantitySoldByDay.GetValueOrDefault(), 
                                 RevenuesByDay =  revenueByDay.GetValueOrDefault(),
                                 TotalImportPriceByDay = totalPaidByDay.GetValueOrDefault()
                            }  }

                        });
                    }

                    revenueReport.TotalSoldQuantity = revenueByDays.Sum(d => d.Values.Sum(t => t.SoldAmountByDay));
                    revenueReport.TotalRevenues = revenueByDays.Sum(d => d.Values.Sum(t => t.RevenuesByDay));
                    revenueReport.Revenues = revenueByDays;


                    var exportByDate = await _context.Invoice.Where(i => i.ExportDate >= request.Filter.StartDate && i.ExportDate <= request.Filter.EndDate)
                        .Include(i => i.InvoiceLine).ToListAsync();

                    revenueReport.TotalVirtualRevenues = exportByDate.Sum(e => e.InvoiceLine.Sum(il => il.ExportPrice * il.ExportAmount)).GetValueOrDefault() - revenueByDays.Sum(d => d.Values.Sum(t => t.TotalImportPriceByDay));
                    revenueReport.TotalExportQuantity = exportByDate.Sum(e => e.InvoiceLine.Sum(il => il.ExportAmount)).GetValueOrDefault();

                    return revenueReport;

                }
                return new RevenuesEnvelope();
                
            }
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


    }
}
