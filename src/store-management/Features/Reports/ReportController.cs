using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using store_management.Features.Reports.Logs;
using store_management.Features.Reports.Products;
using store_management.Features.Reports.Revenues;
using store_management.Features.Reports.Stocks;
using store_management.Infrastructure.Security;

namespace store_management.Features.Reports
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ProductDetails")]
        
        public Task<ReportsEnvelope> List([FromQuery] ProductReportFilter filter)
        {
            return _mediator.Send(new GetProductDetails.Query(filter));
        }

        [HttpGet("Revenues")]
        public Task<RevenuesEnvelope> Get([FromQuery] RevenueParamsFilter filter)
        {
            return _mediator.Send(new GetRevenue.Query(filter));
        }

        [HttpGet("Stocks")]
        public Task<StockEnvelope> Get([FromQuery] StockParemsFilter filter)
        {
            return _mediator.Send(new GetStock.Query(filter));
        }

        [HttpGet("Logs")]
        public Task<LogsEnvelope> Get([FromQuery] LogFilter filter)
        {
            return _mediator.Send(new ListLog.Query(filter));
        }
    }
}