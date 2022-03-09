using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(ProductDto? productDto)
        {
            if (productDto == null)
                return;

            var product = new Product
            {
                Name = productDto.Name,
                Code = productDto.Code,
                UnitPrice = productDto.UnitPrice,
                QuantityPerUnit = productDto.QuantityPerUnit,
                UnitsInStock = productDto.UnitsInStock,
                UnitsInOrder = productDto.UnitsInOrder,
                ReorderLevel = productDto.ReorderLevel,
                Discontinued = productDto.Discontinued,
                Description = productDto.Description
            };

            await _unitOfWork.ProductsRepository.Create(product);
            await _unitOfWork.Commit();
        }

        public async Task<ProductDto?> Get(int productId)
        {
            return await _unitOfWork.ProductsRepository.Get(productId);
        }

        public async Task<ICollection<ProductDto>> GetAll()
        {
            var products = await _unitOfWork.ProductsRepository.GetAll();
            return products.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                UnitPrice = x.UnitPrice,
                Code = x.Code,
                Description = x.Description
            }).ToList();
        }
    }
}
