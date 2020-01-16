using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace store_management.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ProductEnvelope> Edit(string id, [FromBody] Edit.Command command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }

        [HttpDelete]
        public async Task Delete(string id)
        {
            await _mediator.Send(new Delete.Command(id));
        }

    }
}