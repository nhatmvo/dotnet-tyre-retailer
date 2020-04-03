using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;
using store_management.Infrastructure.Errors;
using store_management.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Accounts
{
    public class Create
    {
        public class AccountData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }

        }

        public class AccountDataValidator : AbstractValidator<AccountData>
        {
            public AccountDataValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8);
                
            }
        }

        public class Command : IRequest<AccountEnvelope>
        {
            public AccountData AccountData { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.AccountData).NotNull().SetValidator(new AccountDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, AccountEnvelope>
        {
            private readonly StoreContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly CustomLogger _logger;

            public Handler(StoreContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _currentUserAccessor = currentUserAccessor;
                _logger = new CustomLogger();
            }

            public async Task<AccountEnvelope> Handle(Command command, CancellationToken cancellationToken)
            {
                // Tạo mới tài khoản
                if (await _context.Account.Where(x => x.Username.Equals(command.AccountData.Username)).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = Constants.IN_USE });
                }
                if (string.IsNullOrEmpty(command.AccountData.Role))
                    command.AccountData.Role = "User";

                var salt = Guid.NewGuid().ToByteArray();
                var account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = command.AccountData.Username,
                    Hash = _passwordHasher.Hash(command.AccountData.Password, salt),
                    Salt = salt,
                    Role = command.AccountData.Role
                };

                var claimsIdentity = await _jwtTokenGenerator.GetClaimsIdentity(account.Username, account.Role);
                
                // Ghi log
                var username = _currentUserAccessor.GetCurrentUsername();
                _logger.AddLog(_context, username, username + " thêm mới người dùng " + account.Username, "Thêm mới");

                _context.Account.Add(account);


                await _context.SaveChangesAsync(cancellationToken);
                account.Token = await _jwtTokenGenerator.CreateToken(claimsIdentity);
                return new AccountEnvelope(account);

            }
        }
    }
}
