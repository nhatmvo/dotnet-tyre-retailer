﻿
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
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(string id)
            {
                Id = id;
            }
            public string Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<Command>
        {

            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.Id.ToString().Equals(request.Id), cancellationToken);

                if (customer == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Customer = Constants.NOT_FOUND });
                }

                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

    }
}
