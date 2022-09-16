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
        Task Create(Product product);
        Task<Product?> Get(int productId);
        Task Update(Product product);
        Task Delete(int productId);
    }
}
