using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using store_management.Domain;
using store_management.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using store_management.Infrastructure;
using store_management.Infrastructure.Common;

namespace store_management.Features.Invoices
{
	public class Create
	{
		// Flow muốn export hóa đơn:
		// 1. Chọn lô hàng muốn export
		// 2. Chọn giá và số lượng muốn export
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
			public int InvoiceNo { get; set; }
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
			private readonly CustomLogger _logger;
			private readonly ICurrentUserAccessor _currentUserAccessor;
			private readonly DateTime _now;

			public Handler(StoreContext context, ICurrentUserAccessor currentUserAccessor)
			{
				_context = context;
				_currentUserAccessor = currentUserAccessor;
				_logger = new CustomLogger();
				_now = DateTime.Now;
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
					InvoiceNo = request.InvoiceNo,
					CustomerId = customerId
				};
				var username = _currentUserAccessor.GetCurrentUsername();
				_logger.AddLog(_context, username, username + " xuất hóa đơn mã " + invoice.InvoiceNo + " vào ngày " + _now.ToString(CultureInfo.CurrentCulture), "Tạo mới");
				
				foreach (var item in request.InvoiceLinesData)
				{
					var notBillingProduct = await _context.ProductImport
						.FirstOrDefaultAsync(pe => pe.Id.Equals(item.ProductImportId), cancellationToken);
					
					// điều kiện để export hóa đơn: 
					// Product phải tồn tại
					// Số lượng xuất của hóa đơn phải nhỏ hơn số lượng có thể xuất của sản phẩm
					// Số lượng xuất của hóa đơn phải nhỏ hơn số lượng đã bán của sản phẩm đó (số đã bán mà chưa xuất hóa đơn)
					if (notBillingProduct == null)
						throw new RestException(HttpStatusCode.BadRequest, new { Error = "Lô hàng nhập vào không tồn tại" });
					if (notBillingProduct.ExportableAmount < item.ExportAmount)
						throw new RestException(HttpStatusCode.BadRequest, new { Error = "Số lượng xuất hóa đơn không được nhiều hơn số lượng có thể xuất trong lô" });
					// Check số lượng đã bán của sản phẩm đó
					var product = await _context.Product.Where(p => p.Id.Equals(notBillingProduct.ProductId)).FirstOrDefaultAsync(cancellationToken);
					if (product != null && product.NoBillRemainQuantity < item.ExportAmount)
						throw new RestException(HttpStatusCode.BadRequest, new { Error = "Số lượng xuất hóa đơn không được nhiều hơn số lượng đã bán" });
					if (product != null)
						product.NoBillRemainQuantity -= item.ExportAmount;
					else throw new RestException(HttpStatusCode.NotFound, new { Error = "Không tồn tại sản phẩm trong hệ thống" });
					
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
					
					_logger.AddLog(_context, username, username + " xuất hóa đơn với sản phẩm " + notBillingProduct.Product.Name + ", số lượng " + invoiceLine.ExportAmount + ", với giá " + invoiceLine.ExportPrice + " vào ngày " + _now.ToString(CultureInfo.CurrentCulture) + " tại hóa đơn số " + invoice.InvoiceNo, "Tạo mới");

					// substract no bill remain quantity by export amount
					notBillingProduct.ExportableAmount -= item.ExportAmount;
					_context.ProductImport.Update(notBillingProduct);
					
					
				}

				invoice.Total = lines.Sum(l => l.Total);
				await _context.Invoice.AddAsync(invoice, cancellationToken);
				await _context.InvoiceLine.AddRangeAsync(lines, cancellationToken);
				
				await _context.SaveChangesAsync(cancellationToken);

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
