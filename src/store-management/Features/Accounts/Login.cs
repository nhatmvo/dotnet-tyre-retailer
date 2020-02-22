using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using store_management.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Accounts
{
    public class Login
    {
        public class AccountData
        {
            public string Username { get; set; }
            public string Password { get; set; }

        }

        public class AccountDataValidator : AbstractValidator<AccountData>
        {
            public AccountDataValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();

            }
        }

        public class Command : IRequest<AccountEnvelope>
        {
            public AccountData Account { get; set; }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Account).NotNull().SetValidator(new AccountDataValidator()); 
            }
        }


        public class Handler : IRequestHandler<Command, AccountEnvelope>
        {
            private readonly StoreContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;

            public Handler(StoreContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
            }

            public async Task<AccountEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var account = await _context.Account.Where(x => x.Username == request.Account.Username).SingleOrDefaultAsync(cancellationToken);
                if (account == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Người dùng nhập sai tài khoản / mật khẩu" });

                }

                if (account.Hash.Equals(_passwordHasher.Hash(request.Account.Password, account.Salt)))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Người dùng nhập sai tài khoản / mật khẩu" });
                }

                account.Token = await _jwtTokenGenerator.CreateToken(account.Username);
                return new AccountEnvelope(account);
            }
        }
    }
}
