﻿using store_management.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store_management.Features.Products
{
    public class ProductsEnvelope
    {
        public List<Product> Products { get; set; }
        public int ProductsCount { get; set; }
        public FilterEnvelope AvailableFilter { get; set; }
    }

    public class FilterEnvelope
    {
        public List<string> Brands { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> Patterns { get; set; }
        public List<string> Types { get; set; }
    }

    public enum FilterPriorities
    {
        TYPE = 1, 
        BRAND = 2,
        PATTERN = 3, 
        SIZE = 4
    }

    public class ProductsFilter
    {
        public ProductsFilter()
        {

        }

        public ProductsFilter (string type, string size, string brand, string pattern, int pageSize, int pageIndex, int noBillQuantityGt)
        {
            Type = type;
            Size = size;
            Brand = brand;
            Pattern = pattern;
            PageSize = pageSize;
            PageIndex = pageIndex;
            NoBillQuantityGt = noBillQuantityGt;
            //Limit = limit;
            //Offset = offset;
        }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string Pattern { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public int NoBillQuantityGt { get; set; }
        //public int? Limit { get; set; }
        //public int? Offset { get; set; }

    }
}
