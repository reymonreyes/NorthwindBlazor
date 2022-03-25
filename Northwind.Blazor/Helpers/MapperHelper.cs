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

        public static Supplier? ToSupplier(SupplierDto? supplierDto)
        {
            if (supplierDto is null)
                return null;

            TinyMapper.Bind<SupplierDto, Supplier>();
            return TinyMapper.Map<Supplier>(supplierDto);
        }

        public static SupplierDto? ToSupplierDto(Supplier? supplier)
        {
            if (supplier is null)
                return null;

            TinyMapper.Bind<Supplier, SupplierDto>();
            return TinyMapper.Map<SupplierDto>(supplier);
        }

        public static Shipper? ToShipper(ShipperDto shipperDto)
        {
            if (shipperDto is null)
                return null;

            TinyMapper.Bind<ShipperDto, Shipper>();
            return TinyMapper.Map<Shipper>(shipperDto);
        }

        public static ShipperDto? ToShipperDto(Shipper? shipper)
        {
            if (shipper is null)
                return null;

            TinyMapper.Bind<Shipper, ShipperDto>();
            return TinyMapper.Map<ShipperDto>(shipper);
        }

        public static Customer? ToCustomer(CustomerDto customerDto)
        {
            if (customerDto is null)
                return null;

            TinyMapper.Bind<CustomerDto, Customer>();
            return TinyMapper.Map<Customer>(customerDto);
        }
    }
}
