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

        public static ProductDto? ToProductDto(Product? product)
        {
            if (product is null)
                return null;

            TinyMapper.Bind<Product, ProductDto>();
            return TinyMapper.Map<ProductDto>(product);
        }

        public static CategoryDto? ToCategoryDto(Category? category)
        {
            if (category is null)
                return null;

            TinyMapper.Bind<Category, CategoryDto>();
            return TinyMapper.Map<CategoryDto>(category);
        }

        public static Category? ToCategory(CategoryDto? categoryDto)
        {
            if (categoryDto is null)
                return null;

            TinyMapper.Bind<CategoryDto, Category>();
            return TinyMapper.Map<Category>(categoryDto);
        }
    }
}
