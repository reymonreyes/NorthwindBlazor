using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface IProductsRepository
    {
        Task<ICollection<Product>> GetAll();
        void Create(Product productDto);
        Task<ProductDto?> Get(int productId);
    }
}
