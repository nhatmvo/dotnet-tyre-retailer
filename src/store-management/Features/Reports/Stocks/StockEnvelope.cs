using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Stocks
{
    public class StockParemsFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class StockEnvelope
    {
        public List<ChartData> Data { get; set; }
        public int TotalImports { get; set; }
        public int TotalSold { get; set; }
        public int TotalExports { get; set; }
    }


    public class ChartData
    {
        public string Name { get; set; }
        public List<ChartElement> Series { get; set; }
    }


    public class ChartElement
    {
        public DateTime Name { get; set; }
        public int Value { get; set; }
    }

}
