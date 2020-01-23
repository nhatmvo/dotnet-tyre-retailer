using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace store_management.Features.Invoices
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<InvoiceEnvelope> Create(Create.Command command)
        {
            // Features\Invoices\Create.cs
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<InvoiceEnvelope> Details(string invoiceNo)
        {
            //
            return await _mediator.Send(new Details.Query(invoiceNo));
        }

        [HttpGet("List")]
        public async Task<InvoicesEnvelope> List([FromQuery]List.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}