using Microsoft.AspNetCore.Components;
using Northwind.Blazor.Models;
using Northwind.Core.Interfaces.Services;

namespace Northwind.Blazor.Pages.CustomerOrders
{
    public partial class CustomerOrders
    {
        private IEnumerable<CustomerOrder> _customerOrders;
        private int _page = 1, _totalRecords = 0, _totalPages = 0, _pageSize = 10;
        private bool _isProgressBarVisible;
        [Inject]
        public ICustomerOrdersService CustomerOrdersService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            _isProgressBarVisible = true;
            var orders = await CustomerOrdersService.GetAllAsync(_page);
            var result = orders.Records.Select(x => new CustomerOrder { Id = x.Id, Customer = new Core.Dtos.CustomerDto { Id = x.CustomerId, Name = $"Customer{x.CustomerId}" }, Status = x.Status }).ToList();
            _customerOrders = result;
            _totalRecords = orders.TotalRecords;
            _totalPages = (int)Math.Ceiling(_totalRecords / (double)_pageSize);
            _isProgressBarVisible = false;
        }

        private async Task PaginationChangedAsync(int page)
        {
            _page = page;
            await LoadOrdersAsync();
        }
    }
}
