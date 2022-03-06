using Northwind.Core.Dtos;
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

        public ProductDto Get()
        {
            throw new NotImplementedException();
        }

        public ICollection<ProductDto> GetAll()
        {
            var products = _unitOfWork.ProductsRepository.GetAll();
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
