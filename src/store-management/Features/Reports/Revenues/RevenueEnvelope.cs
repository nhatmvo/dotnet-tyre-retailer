using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Revenues
{
    public class RevenuesEnvelope
    {

        public List<ChartData> Data { get;set; }
        public decimal TotalRevenues { get; set; }
        public decimal TotalVirtualRevenues { get; set; }
        public decimal TotalImportPrice { get; set; }
    }

    public class ChartData
    {
        public string Name { get; set; }
        public List<ChartElement> Series { get; set; }
    }


    public class ChartElement
    {
        public DateTime Name { get; set; }
        public decimal Value { get; set; }
    }


    public class RevenueParamsFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
