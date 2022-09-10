using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface ICategoriesService
    {
        Task<ServiceResult> Create(CategoryDto categoryDto);
        Task<ServiceResult> Update(int categoryId, CategoryDto categoryDto);
        Task<ICollection<CategoryDto>> GetAll();
        Task<CategoryDto?> Get(int categoryId);
        Task Delete(int categoryId);
    }
}
