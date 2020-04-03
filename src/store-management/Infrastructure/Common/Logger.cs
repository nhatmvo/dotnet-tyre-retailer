using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace store_management.Infrastructure.Common
{
    public class CustomLogger
    {
        public CustomLogger() {}
        
        public void AddLog(StoreContext context, string username, string message, string action)
        {
            if (username == null) throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Người dùng không tồn tại trong hệ thống" });
            var currentUserAccount = context.Account.Where(a => a.Username.Equals(username)).FirstOrDefault();
            context.OperationHistory.Add(new OperationHistory
            {
                AccountId = currentUserAccount.Id,
                Action = action,
                ActionDate = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Message = message
            });
        }

    }
}
