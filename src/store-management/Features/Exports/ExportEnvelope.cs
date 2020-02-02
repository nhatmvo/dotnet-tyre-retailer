using store_management.Domain;
using store_management.Features.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Exports
{
    public class ExportEnvelope
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductId { get; set; }
        public string ProductPattern { get; set; }
        public string ProductSize { get; set; }
        public int NoBillRemainQuantity { get; set; }
    }

    public class ExportsEnvelope
    {
        public List<ExportEnvelope> Products { get; set; }
    }
}
