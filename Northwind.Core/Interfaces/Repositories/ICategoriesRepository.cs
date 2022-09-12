using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface ICategoriesRepository
    {
        Task Create(Category category);
        Task<ICollection<Category>> GetAll();
        Task<Category?> Get(int categoryId);
        Task Delete(int categoryId);
        Task Update(Category category);
    }
}
