using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace store_management.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ProductEnvelope> Get(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpPost]
        public async Task<ProductEnvelope> Create([FromBody] Create.Command command)
        {
            var product = new Details.Query(string.Empty, new Details.ProductData
            {
                Brand = command.ProductData.Brand,
                Pattern = command.ProductData.Pattern,
                Size = command.ProductData.Size,
                Type = command.ProductData.Type
            });
            // if product is not null => create update object
            if (product != null)
            {
                var productToUpdate = new Edit.Command
                {
                    Id = product.Id,
                    ProductData = new Edit.ProductData
                    {
                        ImagePath = command.ProductData.ImagePath,
                        ImportPrice = command.ProductData.ImportPrice,
                        Price = command.ProductData.Price,
                        Quantity = command.ProductData.Quantity
                    }
                };
                return await _mediator.Send(productToUpdate);
            }

            return await _mediator.Send(command);
        }

        [HttpPut]
        public async Task<ProductEnvelope> Edit([FromBody] Edit.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete]
        public async Task Delete(string id)
        {
            await _mediator.Send(new Delete.Command(id));
        }

        [HttpGet("List")]
        public async Task<ProductsEnvelope> List([FromQuery] List.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}