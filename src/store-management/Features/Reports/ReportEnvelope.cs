using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Reports
{
    public class ReportEnvelope
    {
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ReportsEnvelope
    {
        public ReportsEnvelope(List<ReportEnvelope> reports)
        {
            Reports = reports;
        }

        public List<ReportEnvelope> Reports { get; set; }
    }
}
