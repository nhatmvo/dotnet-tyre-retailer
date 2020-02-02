using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_management.Features.Products;

namespace store_management.Features.Exports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách những sản phẩm đã bản nhưng chưa được xuất hóa đơn của Product
        /// </summary>
        /// <param name="filter">Tryền vào như bên Filter của Product</param>
        /// <returns></returns>
        [HttpGet]
        public Task<ExportsEnvelope> List([FromQuery] ProductsFilter filter)
        {
            return _mediator.Send(new List.Query(filter));
        } 
    }
}