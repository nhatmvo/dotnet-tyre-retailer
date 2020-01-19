using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace store_management.Features.Transactions
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<TransactionEnvelope> Get(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpPost]
        public async Task<TransactionsEnvelope> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet("List")]
        public async Task<TransactionsEnvelope> List(List.Query query)
        {
            return await _mediator.Send(query);
        }

    }
}