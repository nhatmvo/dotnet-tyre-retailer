using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Features.Importation;

namespace store_management.Features.Importations
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
        public async Task<ImportsEnvelope> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}