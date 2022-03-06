using Nelibur.ObjectMapper;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Common.Helpers
{
    public static class ObjectMapperHelper
    {
        public static ProductDto? ToProductDto(Product? product) 
        {
            if(product is null)
                return null;

            TinyMapper.Bind<Product, ProductDto>();
            return TinyMapper.Map<ProductDto>(product);
        }
    }
}
