using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Infrastructure.Security;

namespace store_management.Features.Sale
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SaleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SaleEnvelope> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<SaleEnvelope> Details(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpGet("List")]
        public async Task<SalesEnvelope> List([FromQuery] List.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}