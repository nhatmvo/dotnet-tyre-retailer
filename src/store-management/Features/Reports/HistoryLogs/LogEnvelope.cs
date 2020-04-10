using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Reports.Logs
{

    public class LogFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }

    public class LogsEnvelope
    {
        public List<OperationHistory> Logs { get; set; }

        public LogsEnvelope(List<OperationHistory> logs)
        {
            Logs = logs;
        }
    }
}
