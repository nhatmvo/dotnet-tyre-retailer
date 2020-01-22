using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Account
    {
        public Account()
        {
            Invoice = new HashSet<Invoice>();
            OperationHistory = new HashSet<OperationHistory>();
            Transaction = new HashSet<Transaction>();
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public virtual ICollection<Invoice> Invoice { get; set; }
        public virtual ICollection<OperationHistory> OperationHistory { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
