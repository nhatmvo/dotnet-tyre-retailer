using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Stocks
{
    public class GetStock
    {
        public class Query : IRequest<StockEnvelope>
        {
            public Query() { }

            public Query(StockParemsFilter filter)
            {
                Filter = filter;
            }

            public StockParemsFilter Filter { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, StockEnvelope>
        {
            private readonly StoreContext _context;

            public QueryHandler(StoreContext context)
            {
                _context = context;
            }

            public async Task<StockEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.Filter != null)
                {
                    var chartLineNames = new List<string> { "Số lượng nhập", "Số lượng bán", "Số lượng xuất" };

                    #region Assign Datetime Value
                    DateTime now = DateTime.Now;
                    // if none of Datetime Filter has value => StartDate and EndDate will be set in a current month
                    if (!(request.Filter.StartDate.HasValue && request.Filter.EndDate.HasValue))
                    {
                        request.Filter.StartDate = (new DateTime(now.Year, now.Month, 1)).AddHours(0).AddMinutes(0).AddSeconds(0);
                        request.Filter.EndDate = request.Filter.StartDate.Value.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    else if (!request.Filter.StartDate.HasValue)
                        request.Filter.StartDate = (new DateTime(now.Year, now.Month, 1)).AddHours(0).AddMinutes(0).AddSeconds(0);
                    else if (!request.Filter.EndDate.HasValue)
                        request.Filter.EndDate = now;
                    #endregion

                    var productInfoByDate = _context.Transaction.Include(p => p.ProductImport).Include(p => p.ProductSale);
                    var stockReport = new StockEnvelope();
                    stockReport.Data = new List<ChartData>();

                    foreach (string lineName in chartLineNames)
                    {
                        var chartData = new ChartData();
                        chartData.Name = lineName;
                        chartData.Series = new List<ChartElement>();

                        foreach (DateTime day in StoreUtils.EachDay(request.Filter.StartDate.Value, request.Filter.EndDate.Value))
                        {
                            var firstDayMoment = day.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                            var lastDayMoment = day.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                            var infoByDay = await productInfoByDate.Where(pbd => pbd.Date >= firstDayMoment && pbd.Date <= lastDayMoment).ToListAsync();
                            var exportByDay = await _context.Invoice.Include(i => i.InvoiceLine).Where(i => i.ExportDate >= firstDayMoment && i.ExportDate <= lastDayMoment).ToListAsync();

                            var chartElement = new ChartElement(); 

                            switch (lineName)
                            {
                                case "Số lượng nhập":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = infoByDay.Sum(i => i.ProductImport.Sum(pi => pi.ImportAmount)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    stockReport.TotalImports += chartElement.Value;
                                    break;
                                case "Số lượng bán":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = infoByDay.Sum(i => i.ProductSale.Sum(pi => pi.SaleAmount)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    stockReport.TotalSold += chartElement.Value;
                                    break;
                                case "Số lượng xuất":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = exportByDay.Sum(i => i.InvoiceLine.Sum(i => i.ExportAmount)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    stockReport.TotalExports += chartElement.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        stockReport.Data.Add(chartData);
                    }
                    

                    return stockReport;

                }
                return new StockEnvelope();
            }
        }
    }
}
