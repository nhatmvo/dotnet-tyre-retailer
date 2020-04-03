using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Customers
{
    public class Edit
    {
        public class CustomerData
        {
            public string FullName { get; set; }
            public string TaxNumber { get; set; }
            public string Address { get; set; }
            public string BankAccountNumber { get; set; }
        }

        public class Command : IRequest<CustomerEnvelope>
        {
            public CustomerData Customer { get; set; }
            public string Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Customer).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, CustomerEnvelope>
        {
            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly Logger _logger;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor, Logger logger)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _logger = new Logger();
            }

            public async Task<CustomerEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.Id.Equals(request.Id), cancellationToken);
                
                if (customer == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Customer = Constants.NOT_FOUND });

                }

                customer.Address = request.Customer.Address ?? customer.Address;
                customer.BankAccountNumber = request.Customer.BankAccountNumber ?? customer.BankAccountNumber;
                customer.FullName = request.Customer.FullName ?? customer.FullName;
                customer.TaxCode = request.Customer.TaxNumber ?? customer.TaxCode;

                _context.Customer.Update(customer);
                var username = _currentUserAccessor.GetCurrentUsername();
                _logger.AddLog(_context, username, username + " sửa thông tin khách hàng " + customer.FullName, "Sửa đổi");

                await _context.SaveChangesAsync();

                return new CustomerEnvelope(customer);
            }
        }
    }
}
