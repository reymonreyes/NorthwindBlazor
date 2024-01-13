using Microsoft.AspNetCore.Components;
using Northwind.Blazor.Models;
using Northwind.Core.Interfaces.Services;
using System.Net.NetworkInformation;

namespace Northwind.Blazor.Pages.PurchaseOrders
{
    public partial class PurchaseOrders
    {
        private IEnumerable<PurchaseOrder> _purchaseOrders;
        private int _page = 1, _totalRecords = 0, _totalPages = 0, _pageSize = 10;
        private bool _isProgressBarVisible;
        [Inject]
        public IPurchaseOrdersService PurchaseOrdersService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            _isProgressBarVisible = true;
            var orders = await PurchaseOrdersService.GetAllAsync(_page);
            var result = orders.Records.Select(x => new PurchaseOrder { Id = x.Id, Supplier = new Core.Dtos.SupplierDto { Id = x.SupplierId, Name = x.SupplierName }, Status = x.Status, Total = x.Total }).ToList();
            _purchaseOrders = result;
            _totalRecords = orders.TotalRecords;
            _totalPages = (int)Math.Ceiling(_totalRecords / (double)_pageSize);
            _isProgressBarVisible = false;
        }

        private async Task PaginationChangedAsync(int  page)
        {
            _page = page;
            await LoadOrdersAsync();
        }
    }
}
