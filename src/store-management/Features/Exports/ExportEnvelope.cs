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
        public string Name { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Pattern { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public decimal? RefPrice { get; set; }
        public int NoBillRemainQuantity { get; set; }
    }

    public class ExportsEnvelope
    {
        public List<ExportEnvelope> Products { get; set; }
    }
}
