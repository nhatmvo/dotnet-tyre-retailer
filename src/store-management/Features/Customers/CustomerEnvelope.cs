using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using store_management.Domain;

namespace store_management.Features.Customers
{
    public class CustomerEnvelope
    {
        public Customer Customer { get; }
        public CustomerEnvelope(Customer customer)
        {
            Customer = customer;
        }
    }
}
