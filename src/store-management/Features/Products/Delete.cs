using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Products
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
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = await _context.Product
                    .FirstOrDefaultAsync(p => p.Id.Equals(request.ToString()) , cancellationToken);
                if (product == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Product = Constants.NOT_FOUND });

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return Unit.Value;
            }
        }
    }
}
