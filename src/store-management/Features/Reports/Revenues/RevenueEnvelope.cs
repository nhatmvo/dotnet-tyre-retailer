using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Revenues
{
    public class RevenuesEnvelope
    {
        public List<Dictionary<DateTime, RevenueByDay>> Revenues { get; set; }
        public decimal TotalRevenues { get; set; }
        public decimal TotalVirtualRevenues { get; set; }
        public int TotalExportQuantity { get; set; }
        public int TotalSoldQuantity { get; set; }
    }

    public class RevenueByDay
    {
        public int SoldAmountByDay { get; set; }
        public decimal RevenuesByDay { get; set; }
        [JsonIgnore]
        public decimal TotalImportPriceByDay { get; set; }
    }

    public class RevenueParamsFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
