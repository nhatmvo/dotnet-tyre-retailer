using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Features.Imports;
using store_management.Infrastructure.Security;

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

        /// <summary>
        /// Tạo mới 1 giá trị nhập hàng của sản phẩm
        /// </summary>
        /// <param name="command">
        ///     Name: tên của sản phẩm
        ///     Type: Loại của sản phẩm (Lốp xe, vành, yếm, ...)
        ///     Brand: Thương hiệu của sản phẩm
        ///     Pattern: 
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ImportEnvelope> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet("Product")]
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