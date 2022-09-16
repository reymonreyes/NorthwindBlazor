using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IProductsRepository ProductsRepository { get; }
        ICategoriesRepository CategoriesRepository { get; }
        ISuppliersRepository SuppliersRepository { get; }
        IShippersRepository ShippersRepository { get; }
        Task Commit();        
        Task Start();
        Task Stop();
    }
}
