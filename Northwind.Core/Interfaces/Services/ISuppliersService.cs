using Northwind.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Services
{
    public interface ISuppliersService
    {
        Task<ICollection<SupplierDto>> GetAll();
        Task<SupplierDto?> Get(int id);
        Task<ServiceResult> Create(SupplierDto? supplierDto);
    }
}
