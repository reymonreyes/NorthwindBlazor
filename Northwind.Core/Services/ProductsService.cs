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

            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", product.Id.ToString()) });
            return result;
        }

        public async Task<ServiceResult> Edit(int id, ProductDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException("product");
            if(id <= 0)
                throw new ArgumentOutOfRangeException("id");

            var product = await _unitOfWork.ProductsRepository.Get(id);
            if (product is null)
                throw new Exception("not found");

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };

            var validationResult = _productValidator.Validate(productDto);
            if (validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                result.Messages.AddRange(validationResult);
            }

            if (!result.IsSuccessful)
                return result;


            product.Name = productDto.Name;
            product.Code = productDto.Code;
            product.UnitPrice = productDto.UnitPrice;
            product.QuantityPerUnit = productDto.QuantityPerUnit;
            product.UnitsInStock = productDto.UnitsInStock;
            product.UnitsInOrder = productDto.UnitsInOrder;
            product.ReorderLevel = productDto.ReorderLevel;
            product.Discontinued = productDto.Discontinued;
            product.Description = productDto.Description;

            await _unitOfWork.Commit();
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", product.Id.ToString()) });

            return result;
        }

        public async Task<ProductDto?> Get(int productId)
        {
            ProductDto? result = null;
            var product = await _unitOfWork.ProductsRepository.Get(productId);
            if(product is not null)
            {
                result = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Code = product.Code,
                    UnitPrice = product.UnitPrice,
                    QuantityPerUnit = product.QuantityPerUnit,
                    UnitsInStock = product.UnitsInStock,
                    UnitsInOrder = product.UnitsInOrder,
                    ReorderLevel = product.ReorderLevel,
                    Discontinued = product.Discontinued,
                    Description = product.Description
                };
            }

            return result;
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
