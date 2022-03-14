using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface IProductsService
    {
        Task<ICollection<ProductDto>> GetAll();
        Task<ProductDto?> Get(int productId);
        Task<ServiceResult> Create(ProductDto? productDto);
        Task Edit(int id, ProductDto productDto);
    }
}
