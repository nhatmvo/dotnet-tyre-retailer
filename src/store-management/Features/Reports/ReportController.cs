using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static store_management.Features.Reports.ProdDetails;

namespace store_management.Features.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<ReportsEnvelope> List([FromQuery] ProductReportFilter filter)
        {
            return _mediator.Send(new ProdDetails.Query(filter));
        }
    }
}