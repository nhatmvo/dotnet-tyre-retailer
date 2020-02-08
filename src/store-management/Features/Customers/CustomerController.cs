using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Infrastructure;

namespace store_management.Features.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public CustomerController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
        {
            _mediator = mediator;
            _currentUserAccessor = currentUserAccessor;
        }

        /// <summary>
        /// Lấy thông tin của khách hàng dựa vào taxCode có từ trước
        /// </summary>
        /// <param name="taxCode">taxCode của khách hàng</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CustomerEnvelope> Get([FromQuery] string taxCode)
        {
            return await _mediator.Send(new Details.Query(taxCode));
        }

        [HttpPost]
        public async Task<CustomerEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut]
        public async Task<CustomerEnvelope> Edit(string id, [FromBody] Edit.Command command)
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