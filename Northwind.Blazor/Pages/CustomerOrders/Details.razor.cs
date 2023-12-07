using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Northwind.Blazor.Models;
using Northwind.Blazor.Models.Validators;
using Northwind.Common.Extensions;
using Northwind.Common.Enums;
using Northwind.Core.Dtos;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Services;
using System.Reflection.Metadata.Ecma335;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Northwind.Blazor.Pages.CustomerOrders
{
    public partial class Details
    {
        public EntryMode _entryMode = EntryMode.Create;
        public CustomerOrder? _customerOrder = null;
        public CustomerOrderValidator _customerOrderValidator = new CustomerOrderValidator();
        public MudForm? _customerOrderForm;
        public bool _isCustomerOrderFormValid, _isCustomerOrderFormOverlayVisible;
        public List<string> _errors = new List<string>();
        
        [Inject]
        public ICustomerOrdersService CustomerOrdersService { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; }
        [Inject]
        public IShippersService ShippersService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int Id { get; set; }

        public Details()
        {            
        }
        protected override async Task OnInitializedAsync()
        {
            _entryMode = NavigationManager.Uri.EndsWith("create") ? EntryMode.Create : EntryMode.Edit;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    NewCustomerOrder();
                    break;
                case EntryMode.Edit:
                    await LoadCustomerOrder();
                    break;
            }
        }

        private void NewCustomerOrder()
        {
            _customerOrder = new CustomerOrder();
            _customerOrder.OrderDate = DateTime.Now;
        }

        private async Task LoadCustomerOrder()
        {
            _isCustomerOrderFormOverlayVisible = true;
            var customerOrder = await CustomerOrdersService.GetAsync(Id);
            _isCustomerOrderFormOverlayVisible = false;
            if (customerOrder is null)
            {
                Notify("Customer Order not found", MudBlazor.Severity.Error);
                return;
            }

            _customerOrder = new CustomerOrder
            {
                Id = customerOrder.Id,
                CustomerId = customerOrder.CustomerId,
                OrderDate = customerOrder.OrderDate,
                DueDate = customerOrder.DueDate,
                ShipDate = customerOrder.ShipDate,
                ShipperId = customerOrder.ShipperId,
                Notes = customerOrder.Notes
            };

            var customer = await CustomersService.Get(_customerOrder.CustomerId);
            if(customer is not null)
                _customerOrder.Customer = new CustomerDto { Id = customer.Id, Name = customer.Name };

            Console.WriteLine("CustomerOrder loaded");
        }

        private async Task Save()
        {
            await _customerOrderForm.Validate();
            if (!_isCustomerOrderFormValid)
                return;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    await Create();
                    break;
                case EntryMode.Edit:
                    break;
            }
        }

        private async Task Create()
        {
            _errors = new List<string>();
            try
            {
                _isCustomerOrderFormOverlayVisible = true;
                var createResult = await CustomerOrdersService.Create(_customerOrder.Customer.Id, _customerOrder.OrderDate, new List<Core.Dtos.CustomerOrderItemDto>());
                if(createResult.IsSuccessful)
                {
                    var id = createResult?.Messages?.FirstOrDefault()?.Message.Value;
                    Id = int.Parse(id);
                    _entryMode = EntryMode.Edit;
                    NavigationManager.NavigateTo($"customerorders/{id}", false, true);
                    Notify($"Customer Order # {id} {(_entryMode == EntryMode.Create ? "created" : "updated")} successfully.", MudBlazor.Severity.Success);
                }
            }
            catch (ValidationFailedException exc)
            {
                _errors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
            }
            catch (Exception exc)
            {
                _errors.Add(exc.Message);
            }
            finally
            {
                _isCustomerOrderFormOverlayVisible = false;
            }

        }

        private async Task<IEnumerable<CustomerDto>> FindCustomer(string customerName)
        {
            if(string.IsNullOrWhiteSpace(customerName) || (!string.IsNullOrWhiteSpace(customerName) && customerName.Length < 2))
                return new List<CustomerDto>();

            var customers = await CustomersService.FindAsync(customerName);

            return customers.Select(x => new CustomerDto { Id = x.Id, Name = x.Name}).ToList();
        }

        private async Task<IEnumerable<ShipperDto>> FindShipper(string shipperName)
        {
            if (shipperName.IsEmptyOrLengthLessThan(2))
                return new List<ShipperDto>();

            var shippers = await ShippersService.FindAsync(shipperName);

            return shippers;
        }
    }

    public class CustomerOrderValidator : BaseValidator<CustomerOrder>
    {
        public CustomerOrderValidator()
        {
            RuleFor(x => x.Customer).NotEmpty().WithMessage("Customer is required");
            RuleFor(x => x.OrderDate).NotEmpty().WithMessage("OrderDate is required");
        }
    }
}
