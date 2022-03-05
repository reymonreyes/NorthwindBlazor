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
        ICollection<ProductDto> GetAll();
        ProductDto Get();
    }
}
