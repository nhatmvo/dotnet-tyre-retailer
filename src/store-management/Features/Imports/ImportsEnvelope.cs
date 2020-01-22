using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Importation
{
    public class ImportsEnvelope
    {
        public List<Product> Products { get; set; }
    }
}
