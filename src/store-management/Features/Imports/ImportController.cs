using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Features.Imports;

namespace store_management.Features.Imports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ImportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ImportEnvelope> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<ImportEnvelope> Details(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpGet("List")]
        public async Task<ImportsEnvelope> List([FromQuery] List.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}