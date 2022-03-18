using Northwind.Core.Dtos;
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
    }
}
