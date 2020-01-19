using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
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

            public Handler(StoreContext context)
            {
                _context = context;
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
                await _context.SaveChangesAsync();

                return new CustomerEnvelope(customer);
            }
        }
    }
}
