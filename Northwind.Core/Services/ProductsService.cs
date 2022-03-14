using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
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
        private readonly IProductValidator _productValidator;
        public ProductsService(IUnitOfWork unitOfWork, IProductValidator productValidator)
        {
            _unitOfWork = unitOfWork;
            _productValidator = productValidator;

        }

        public async Task<ServiceResult> Create(ProductDto? productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException("product");

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };

            var validationResult = _productValidator.Validate(productDto);
            if(validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                result.Messages.AddRange(validationResult);
            }

            if (!result.IsSuccessful)
                return result;

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

            return result;
        }

        public async Task Edit(int id, ProductDto productDto)
        {
            if (productDto == null)
                return;

            await _unitOfWork.ProductsRepository.Create(new Product { Id = id });
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
