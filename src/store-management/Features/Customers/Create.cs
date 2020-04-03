using FluentValidation;
using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;

namespace store_management.Features.Customers
{
    public class Create
    {
        public class CustomerData
        {
            public string FullName { get; set; }    
            public string Address { get; set; }
            public string BankAccount { get; set; }
            public string TaxNumber { get; set; }
        }

        public class CustomerValidator : AbstractValidator<CustomerData>
        {
            public CustomerValidator()
            {
                RuleFor(x => x.FullName).NotNull().NotEmpty();
                RuleFor(x => x.Address).NotNull().NotEmpty();
                RuleFor(x => x.TaxNumber).NotNull().NotEmpty().Length(10);
            }
        }

        public class Command : IRequest<CustomerEnvelope>
        {
            public CustomerData Customer { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Customer).NotNull().SetValidator(new CustomerValidator());
            }
        }

        public class Handler : IRequestHandler<Command, CustomerEnvelope>
        {

            private readonly StoreContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly Logger _logger;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _logger = new Logger();
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }


            public async Task<CustomerEnvelope> Handle(Command command, CancellationToken cancellationToken)
            {

                var existedCustomer = await _context.Customer.FirstOrDefaultAsync(c => c.TaxCode.Equals(command.Customer.TaxNumber));
                if (existedCustomer != null)
                    throw new RestException(HttpStatusCode.BadRequest, new { });
                var customer = new Customer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Address = command.Customer.Address,
                    TaxCode = command.Customer.TaxNumber,
                    BankAccountNumber = command.Customer.BankAccount,
                    FullName = command.Customer.FullName
                };
                // Ghi log
                var username = _currentUserAccessor.GetCurrentUsername();
                _logger.AddLog(_context, username, username + " thêm mới khách hàng " + customer.FullName, "Thêm mới");
                await _context.Customer.AddAsync(customer, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new CustomerEnvelope(customer);
            }
        }
    }
}
