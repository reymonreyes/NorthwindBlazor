using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class SuppliersService : ISuppliersService
    {
        public Task<ICollection<SupplierDto>> GetAll()
        {
            var data = new List<SupplierDto>
                    {
                        new SupplierDto{ Id = 1, Name = "Alpha One"},
                        new SupplierDto{ Id = 2, Name = "Bravo Two" },
                        new SupplierDto{ Id = 3, Name = "Charlie Three" },
                        new SupplierDto{ Id = 4, Name = "Delta Four" },
                        new SupplierDto{ Id = 5, Name = "Eagle Five" }
                    };

            return Task.FromResult<ICollection<SupplierDto>>(data);
        }
    }
}
