using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class Customer
    {
        public Customer()
        {
            Invoice = new HashSet<Invoice>();
        }

        public string Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string BankAccountNumber { get; set; }
        public string TaxCode { get; set; }

        public virtual ICollection<Invoice> Invoice { get; set; }
    }
}
