using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Infrastructure.Security;

namespace store_management.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ProductEnvelope> Get(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpPost]
        public async Task<ProductEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut]
        public async Task<ProductEnvelope> Edit([FromBody] Edit.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task Delete(string id)
        {
            await _mediator.Send(new Delete.Command(id));
        }

        [HttpGet("List")]
        public async Task<ProductsEnvelope> List([FromQuery] List.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}