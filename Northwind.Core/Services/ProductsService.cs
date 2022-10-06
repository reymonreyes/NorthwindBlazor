using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public async Task<ServiceResult> Create(ProductDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException("productDto");

            Validate(productDto);

            var product = new Product
            {
                Name = productDto.Name,
                Code = productDto.Code,
                ListPrice = productDto.UnitPrice,
                QuantityPerUnit = productDto.QuantityPerUnit,
                //UnitsInStock = productDto.UnitsInStock, - revisit
                //UnitsInOrder = productDto.UnitsInOrder, - revisit
                ReorderLevel = productDto.ReorderLevel,
                Discontinued = productDto.Discontinued,
                Description = productDto.Description
            };

            await _unitOfWork.Start();
            await _unitOfWork.ProductsRepository.Create(product);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", product.Id.ToString()) });
            return result;
        }

        private void Validate(ProductDto productDto)
        {
            var validationResult = _productValidator.Validate(productDto);
            if (validationResult?.Count > 0)
                throw new ValidationFailedException(validationResult);
        }

        public async Task<ServiceResult> Update(int id, ProductDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException("product");
            if(id <= 0)
                throw new ArgumentOutOfRangeException("id");

            Validate(productDto);

            await _unitOfWork.Start();
            var product = await _unitOfWork.ProductsRepository.Get(id);
            if (product is null)
                throw new DataNotFoundException("Product not found.");

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            
            product.Name = productDto.Name;
            product.Code = productDto.Code;
            //product.UnitPrice = productDto.UnitPrice; - revisit
            product.QuantityPerUnit = productDto.QuantityPerUnit;
            //product.UnitsInStock = productDto.UnitsInStock; - revisit
            //product.UnitsInOrder = productDto.UnitsInOrder; - revisit
            product.ReorderLevel = productDto.ReorderLevel;
            product.Discontinued = productDto.Discontinued;
            product.Description = productDto.Description;
            
            await _unitOfWork.ProductsRepository.Update(product);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", product.Id.ToString()) });

            return result;
        }

        public async Task<ProductDto?> Get(int productId)
        {
            ProductDto? result = null;
            await _unitOfWork.Start();
            var product = await _unitOfWork.ProductsRepository.Get(productId);
            await _unitOfWork.Stop();

            if(product is not null)
            {
                result = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Code = product.Code,
                    //UnitPrice = product.UnitPrice,-revisit
                    QuantityPerUnit = product.QuantityPerUnit,
                    //UnitsInStock = product.UnitsInStock,-revisit
                    //UnitsInOrder = product.UnitsInOrder,-revisit
                    ReorderLevel = product.ReorderLevel,
                    Discontinued = product.Discontinued,
                    Description = product.Description
                };
            }

            return result;
        }

        public async Task<ICollection<ProductDto>> GetAll()
        {
            await _unitOfWork.Start();
            var products = await _unitOfWork.ProductsRepository.GetAll();
            await _unitOfWork.Stop();

            return products.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                //UnitPrice = x.UnitPrice,revisit
                Code = x.Code,
                Description = x.Description
            }).ToList();
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.Start();
            await _unitOfWork.ProductsRepository.Delete(id);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }
    }
}
