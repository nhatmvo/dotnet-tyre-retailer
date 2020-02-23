using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace store_management.Domain
{
    public partial class ProductExport
    {
        public string Id { get; set; }
        public string ProductImportId { get; set; }
        public int? NoBillRemainQuantity { get; set; }

        [JsonIgnore]
        public virtual ProductImport ProductImport { get; set; }
    }
}
