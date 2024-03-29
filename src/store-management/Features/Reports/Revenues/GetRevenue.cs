﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Common;
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
                    #region Assign Datetime Value
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
                    #endregion

                    var chartLineNames = new List<string> { "Doanh thu", "Doanh thu ảo", "Giá nhập" };

                    var productInfoByDate = _context.Transaction.Include(p => p.ProductImport).Include(p => p.ProductSale);

                    var revenueReport = new RevenuesEnvelope();
                    revenueReport.Data = new List<ChartData>();

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
                            var exportByDay = await _context.Invoice.Where(i => i.ExportDate >= firstDayMoment && i.ExportDate <= lastDayMoment).ToListAsync();

                            var chartElement = new ChartElement();
                            switch (lineName)
                            {
                                case "Doanh thu":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = infoByDay.Sum(i => i.ProductSale.Sum(pi => pi.SaleAmount * pi.SalePrice)).GetValueOrDefault() -
                                            infoByDay.Sum(i => i.ProductImport.Sum(pi => pi.ImportAmount * pi.ImportPrice)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    revenueReport.TotalRevenues += chartElement.Value;
                                    break;
                                case "Doanh thu ảo":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = exportByDay.Sum(i => i.InvoiceLine.Sum(i => i.ExportAmount * i.ExportPrice)).GetValueOrDefault() -
                                            infoByDay.Sum(i => i.ProductImport.Sum(pi => pi.ImportAmount * pi.ImportPrice)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    revenueReport.TotalVirtualRevenues += chartElement.Value;
                                    break;
                                case "Giá nhập":
                                    chartElement = new ChartElement()
                                    {
                                        Name = day,
                                        Value = infoByDay.Sum(i => i.ProductImport.Sum(i => i.ImportAmount * i.ImportPrice)).GetValueOrDefault()
                                    };
                                    chartData.Series.Add(chartElement);
                                    revenueReport.TotalImportPrice += chartElement.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        revenueReport.Data.Add(chartData);
                    }
                    
                    return revenueReport;

                }
                return new RevenuesEnvelope();
                
            }
        }




    }
}
