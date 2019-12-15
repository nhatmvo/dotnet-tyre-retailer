using FluentValidation;
using MediatR;
using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            public CustomerData Customer;
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
            //private readonly 

            public Handler(StoreContext context)
            {
                _context = context;
            }


            public async Task<CustomerEnvelope> Handle(Command command, CancellationToken cancellationToken)
            {
                var customer = new Customer()
                {
                    Id = Guid.NewGuid().ToByteArray(),
                    Address = command.Customer.Address,
                    TaxCode = command.Customer.TaxNumber,
                    BankAccountNumber = command.Customer.BankAccount,
                    FullName = command.Customer.FullName
                };

                await _context.Customer.AddAsync(customer, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new CustomerEnvelope(customer);
            }
        }
    }
}
