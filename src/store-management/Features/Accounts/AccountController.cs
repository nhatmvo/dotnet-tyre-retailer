using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using store_management.Infrastructure.Security;

namespace store_management.Features.Accounts
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        [Authorize(Roles = "Admin")]
        public async Task<AccountEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }


        [HttpPost("Login")]
        public async Task<AccountEnvelope> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}