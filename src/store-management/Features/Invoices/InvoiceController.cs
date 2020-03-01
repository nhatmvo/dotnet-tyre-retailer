using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Infrastructure.Security;

namespace store_management.Features.Invoices
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Xuất hóa đơn dựa vào mã sản phẩm (số lượng xuất hóa đơn nhỏ hơn số lượng chưa xuát hóa đơn)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
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