using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Accounts
{
    public class AccountEnvelope
    {
        public AccountEnvelope(Account account)
        {
            Account = account;
        }

        public Account Account { get; set; }
    }
}
