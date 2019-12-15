using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class OperationHistory
    {
        public byte[] Id { get; set; }
        public byte[] AccountId { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public DateTime? ActionDate { get; set; }

        public virtual Account Account { get; set; }
    }
}
