using Nelibur.ObjectMapper;
using Northwind.Blazor.Models;
using Northwind.Core.Dtos;

namespace Northwind.Blazor.Helpers
{
    public static class MapperHelper
    {
        public static Product? ToProduct(ProductDto? productDto)
        {
            if (productDto is null)
                return null;

            TinyMapper.Bind<ProductDto, Product>();
            return TinyMapper.Map<Product>(productDto);
        }
    }
}
