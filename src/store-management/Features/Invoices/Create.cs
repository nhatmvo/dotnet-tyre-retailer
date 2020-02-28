using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace store_management.Features.Invoices
{
	public class Create
	{
		public class InvoiceData
		{
			public string ProductImportId { get; set; }
			public int ExportAmount { get; set; }
			public decimal? ExportPrice { get; set; }
		}

		public class CustomerData
		{
			public string FullName { get; set; }
			public string Address { get; set; }
			public string BankAccountNumber { get; set; }
			public string TaxCode { get; set; }
		}

		public class InvoiceDataValidator : AbstractValidator<InvoiceData>
		{
			public InvoiceDataValidator()
			{
				RuleFor(x => x.ProductImportId).NotNull().NotEmpty();
			}
		}

		public class Command : IRequest<InvoiceEnvelope>
		{
			public List<InvoiceData> InvoiceLinesData { get; set; }
			public CustomerData CustomerData { get; set; }
		}

		public class CommandValidator : AbstractValidator<Command>
		{
			public CommandValidator()
			{
				RuleForEach(x => x.InvoiceLinesData).NotNull().NotEmpty()
					.SetValidator(new InvoiceDataValidator());
			}
		}

		public class Handler : IRequestHandler<Command, InvoiceEnvelope>
		{
			private readonly StoreContext _context;

			public Handler(StoreContext context)
			{
				_context = context;
			}

			public async Task<InvoiceEnvelope> Handle(Command request, CancellationToken cancellationToken)
			{
				var lines = new List<InvoiceLine>();

				var existedCustomer = await HandleExistedCustomer(request.CustomerData.TaxCode);
				var customerId = Guid.NewGuid().ToString();
				if (existedCustomer == null)
				{
					var customer = new Customer()
					{
						Address = request.CustomerData.Address,
						BankAccountNumber = request.CustomerData.BankAccountNumber,
						FullName = request.CustomerData.FullName,
						Id = customerId,
						TaxCode = request.CustomerData.TaxCode
					};
					await _context.Customer.AddAsync(customer);
				}
				customerId = existedCustomer != null ? existedCustomer.Id : customerId;

				var invoice = new Invoice()
				{
					Id = Guid.NewGuid().ToString(),
					ExportDate = DateTime.Now,
					InvoiceNo = (new Random()).Next(10000000, 99999999),
					CustomerId = customerId
				};


				foreach (var item in request.InvoiceLinesData)
				{

					var notBillingProduct = await _context.ProductImport
						.FirstOrDefaultAsync(pe => pe.Id.Equals(item.ProductImportId));

					if (notBillingProduct == null)
						throw new RestException(HttpStatusCode.BadRequest, new { });
					if (notBillingProduct.ExportableAmount < item.ExportAmount)
						throw new RestException(HttpStatusCode.BadRequest, new { });

					var exportPrice = item.ExportPrice ?? notBillingProduct.ImportPrice;
					var invoiceLine = new InvoiceLine()
					{
						ExportPrice = exportPrice,
						Id = Guid.NewGuid().ToString(),
						InvoiceId = invoice.Id,
						ExportAmount = item.ExportAmount,
						Total = exportPrice * item.ExportAmount,
						ProductId = notBillingProduct.ProductId
					};
					lines.Add(invoiceLine);

					// substract no bill remain quantity by export amount
					notBillingProduct.ExportableAmount -= item.ExportAmount;
					_context.ProductImport.Update(notBillingProduct);
				}

				invoice.Total = lines.Sum(l => l.Total);
				await _context.Invoice.AddAsync(invoice);
				await _context.InvoiceLine.AddRangeAsync(lines);

				await _context.SaveChangesAsync();

				return new InvoiceEnvelope(invoice);
			}

			private async Task<Customer> HandleExistedCustomer(string taxCode)
			{
				var customer = await _context.Customer.AsNoTracking()
				   .FirstOrDefaultAsync(c => c.TaxCode.ToLower().Equals(taxCode));
				return customer;
			}
		}
	}
}
