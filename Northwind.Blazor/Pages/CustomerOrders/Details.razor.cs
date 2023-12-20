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
        public CustomerOrderItem _customerOrderItem = new CustomerOrderItem();
        public CustomerOrderValidator _customerOrderValidator = new CustomerOrderValidator();
        public CustomerOrderItemValidator _customerOrderItemValidator = new CustomerOrderItemValidator();
        public MudForm? _customerOrderForm, _addItemForm;
        public bool _isCustomerOrderFormValid, _isCustomerOrderFormOverlayVisible, _isCustomerOrderItemFormValid, _isCustomerOrderItemFormOverlayVisible;
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

        private async Task<bool> Save()
        {
            await _customerOrderForm.Validate();
            if (!_isCustomerOrderFormValid)
                return false;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    await Create();
                    break;
                case EntryMode.Edit:
                    break;
            }

            return true;
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
                    Notify($"Customer Order # {id} {(_entryMode == EntryMode.Create ? "created" : "updated")} successfully.", MudBlazor.Severity.Success);
                    Id = int.Parse(id);
                    _customerOrder.Id = Id;
                    _entryMode = EntryMode.Edit;
                    NavigationManager.NavigateTo($"customerorders/{id}", false, true);
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

        private async Task AddItemAsync()
        {
            await _addItemForm.Validate();
            if (!_isCustomerOrderItemFormValid)
                return;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    var successful = await Save();
                    if (successful)
                        await AddItemToOrder();
                    break;
                case EntryMode.Edit:
                    await AddItemToOrder();
                    break;
            }
        }

        private async Task AddItemToOrder()
        {
            var item = new CustomerOrderItemDto
            {
                ProductId = _customerOrderItem.ProductId,
                Quantity = _customerOrderItem.Qty,
                UnitPrice = _customerOrderItem.UnitPrice,
            };

            try
            {
                _isCustomerOrderItemFormOverlayVisible = true;
                await CustomerOrdersService.AddItem(_customerOrder.Id, item);
                _customerOrder.Items.Add(_customerOrderItem);
                _customerOrderItem = new CustomerOrderItem();
                _customerOrderForm.Reset();
                Notify($"Item added successfully.", MudBlazor.Severity.Success);
            }
            catch (Exception exc)
            {
                var message = exc.Message;
                Notify(message, MudBlazor.Severity.Error);
            }
            _isCustomerOrderItemFormOverlayVisible = false;
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

    public class CustomerOrderItemValidator : BaseValidator<CustomerOrderItem>
    {
        public CustomerOrderItemValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Required");
            RuleFor(x => x.Qty).NotEmpty().WithMessage("Required").GreaterThan(0).WithMessage("Invalid");
            RuleFor(x => x.UnitPrice).NotEmpty().WithMessage("Required").GreaterThan(0).WithMessage("Invalid");
        }
    }
}
