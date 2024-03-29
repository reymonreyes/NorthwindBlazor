﻿using FluentValidation;
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
using Northwind.Core.Entities;
using Northwind.Core.Services;
using Northwind.Blazor.Helpers;
using Microsoft.JSInterop;

namespace Northwind.Blazor.Pages.CustomerOrders
{
    public partial class Details
    {
        private EntryMode _entryMode = EntryMode.Create;
        private Models.CustomerOrder _customerOrder = new Models.CustomerOrder();
        private Models.CustomerOrderItem _customerOrderItem = new Models.CustomerOrderItem();
        private CustomerOrderValidator _customerOrderValidator = new CustomerOrderValidator();
        private CustomerOrderItemValidator _customerOrderItemValidator = new CustomerOrderItemValidator();
        private MudForm? _customerOrderForm, _addItemForm;
        private bool _isCustomerOrderFormValid, _isCustomerOrderFormOverlayVisible, _isCustomerOrderItemFormValid, _isCustomerOrderItemFormOverlayVisible;
        private List<string> _errors = new List<string>();
        private MudAutocomplete<ProductDto> _productAutocomplete;
        
        [Inject]
        public ICustomerOrdersService CustomerOrdersService { get; set; }
        [Inject]
        public ICustomersService CustomersService { get; set; }
        [Inject]
        public IShippersService ShippersService { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IDialogService DialogService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
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
            _customerOrder = new Models.CustomerOrder();
            _customerOrder.OrderDate = DateTime.Now;
        }

        private async Task LoadCustomerOrder()
        {
            _isCustomerOrderFormOverlayVisible = true;
            _isCustomerOrderItemFormOverlayVisible = true;
            var customerOrder = await CustomerOrdersService.GetAsync(Id, true);
            if (customerOrder is null)
            {
                Notify("Customer Order not found", MudBlazor.Severity.Error);
                return;
            }

            _customerOrder = new Models.CustomerOrder
            {
                Id = customerOrder.Id,
                CustomerId = customerOrder.CustomerId,
                Customer = new CustomerDto { Id = customerOrder.CustomerId, Name = customerOrder.CustomerName },
                OrderDate = customerOrder.OrderDate,
                DueDate = customerOrder.DueDate,
                ShipDate = customerOrder.ShipDate,
                ShipperId = customerOrder.ShipperId,
                Notes = customerOrder.Notes,
                Status = customerOrder.Status,
                Items = customerOrder.Items.Select(x => new Models.CustomerOrderItem { Id = x.Id, Product = new ProductDto { Id = x.ProductId, Name = x.ProductName }, Qty = x.Quantity, UnitPrice = x.UnitPrice.Value }).ToList()
            };

            var customer = await CustomersService.Get(_customerOrder.CustomerId);
            if(customer is not null)
                _customerOrder.Customer = new CustomerDto { Id = customer.Id, Name = customer.Name };

            _isCustomerOrderFormOverlayVisible = false;
            _isCustomerOrderItemFormOverlayVisible = false;
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
                    await Update();
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

        private async Task Update()
        {
            try
            {
                _errors.Clear();
                _isCustomerOrderFormOverlayVisible = true;
                var result = await CustomerOrdersService.Update(Id, _customerOrder.ToCustomerOrderDto());
                if (result.IsSuccessful)
                    Notify($"Customer Order # {Id} {(_entryMode == EntryMode.Create ? "created" : "updated")} successfully.", MudBlazor.Severity.Success);
            }
            catch (ValidationFailedException exc)
            {
                if (exc.ValidationErrors.Any())
                    _errors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
                else
                    _errors.Add(exc.Message);
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
            var itemExist = _customerOrder.Items.FirstOrDefault(x => x.Product.Id == _customerOrderItem.Product.Id);
            if(itemExist is not null)
            {
                itemExist.Qty += _customerOrderItem.Qty;
                await UpdateItemAsync(itemExist);
                _addItemForm.Reset();
                await _productAutocomplete.Clear();
                return;
            }

            var item = new CustomerOrderItemDto
            {
                ProductId = _customerOrderItem.Product.Id,
                Quantity = _customerOrderItem.Qty,
                UnitPrice = _customerOrderItem.UnitPrice,
            };

            try
            {
                _isCustomerOrderItemFormOverlayVisible = true;
                await CustomerOrdersService.AddItem(_customerOrder.Id, item);
                _customerOrder.Items.Add(_customerOrderItem);
                _customerOrderItem = new Models.CustomerOrderItem();
                _addItemForm.Reset();
                await _productAutocomplete.Clear();
                Notify($"Item added successfully.", MudBlazor.Severity.Success);
            }
            catch (Exception exc)
            {
                var message = exc.Message;
                Notify(message, MudBlazor.Severity.Error);
            }
            _isCustomerOrderItemFormOverlayVisible = false;
        }

        private async Task<IEnumerable<ProductDto>> FindProducts(string productName)
        {
            if(string.IsNullOrEmpty(productName))
                return new List<ProductDto>();

            ICollection<ProductDto> matchedProducts = await ProductsService.Find(productName);

            return matchedProducts;
        }

        private void OnProductValueChanged(ProductDto? productDto)
        {
            Console.WriteLine("ValueChanged");
            _customerOrderItem.Product = productDto;
            if (productDto is not null)
                _customerOrderItem.UnitPrice = productDto.ListPrice;
        }

        private void EditItem(Models.CustomerOrderItem customerOrderItem)
        {
            Console.WriteLine("EDITITEM");
            customerOrderItem.IsInEditMode = true;
        }

        private void CancelUpdateItem(Models.CustomerOrderItem customerOrderItem)
        {
            Console.WriteLine("CancelUpdateItem from parent");
            customerOrderItem.IsInEditMode = false;
            customerOrderItem.EditItem = null;
            StateHasChanged();
        }

        private async Task UpdateItemAsync(Models.CustomerOrderItem customerOrderItem)
        {
            _isCustomerOrderItemFormOverlayVisible = true;

            var item = new CustomerOrderItemDto
            {
                Id = customerOrderItem.Id,
                ProductId = customerOrderItem.Product.Id,
                Quantity = customerOrderItem.Qty,
                UnitPrice = customerOrderItem.UnitPrice,
                CustomerOrderId = _customerOrder.Id
            };

            try
            {
                await CustomerOrdersService.UpdateItem(Id, item);
                Notify($"Item updated successfully.", MudBlazor.Severity.Success);
            }
            catch (Exception exc)
            {
                var message = exc.Message;
                Notify(message, MudBlazor.Severity.Error);
            }
            finally
            {
                customerOrderItem.IsInEditMode = false;
                customerOrderItem.EditItem = null;
                _isCustomerOrderItemFormOverlayVisible = false;
            }
        }

        private async Task RemoveItem(Models.CustomerOrderItem customerOrderItem)
        {
            _isCustomerOrderItemFormOverlayVisible = true;
            _customerOrder.Items.Remove(customerOrderItem);
            await CustomerOrdersService.RemoveItem(customerOrderItem.Id);
            _isCustomerOrderItemFormOverlayVisible = false;
        }

        private async Task Cancel()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Cancel",
                "Are you sure you want to cancel this Customer Order?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    _isCustomerOrderFormOverlayVisible = true;
                    _isCustomerOrderItemFormOverlayVisible = true;
                    await CustomerOrdersService.CancelAsync(Id);
                    _customerOrder.Status = Core.Enums.OrderStatus.Cancelled;
                    _isCustomerOrderFormOverlayVisible = false;
                    _isCustomerOrderItemFormOverlayVisible = false;
                    Notify("Customer Order cancelled.", MudBlazor.Severity.Warning);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
            }
        }

        private async Task MarkAsInvoiced()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Invoiced",
                "Do you want to mark this Customer Order as Invoiced?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    _isCustomerOrderFormOverlayVisible = true;
                    _isCustomerOrderItemFormOverlayVisible = true;
                    await CustomerOrdersService.MarkAsInvoiced(Id);
                    _customerOrder.Status = Core.Enums.OrderStatus.Invoiced;
                    Notify("Customer Order invoiced.", MudBlazor.Severity.Success);
                }
                catch(ValidationFailedException exc)
                {
                    var validationErrors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
                    var errorMessage = string.Join("<br/>", validationErrors);
                    Notify(errorMessage, MudBlazor.Severity.Error);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
                finally
                {
                    _isCustomerOrderFormOverlayVisible = false;
                    _isCustomerOrderItemFormOverlayVisible = false;
                }
            }
        }

        private async Task MarkAsShippedAsync()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Invoiced",
                "Do you want to mark this Customer Order as Shipped?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    _isCustomerOrderFormOverlayVisible = true;
                    _isCustomerOrderItemFormOverlayVisible = true;
                    await CustomerOrdersService.MarkAsShipped(Id);
                    _customerOrder.Status = Core.Enums.OrderStatus.Shipped;
                    Notify("Customer Order shipped.", MudBlazor.Severity.Success);
                }
                catch (ValidationFailedException exc)
                {
                    var validationErrors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
                    var errorMessage = string.Join("<br/>", validationErrors);
                    Notify(errorMessage, MudBlazor.Severity.Error);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
                finally
                {
                    _isCustomerOrderFormOverlayVisible = false;
                    _isCustomerOrderItemFormOverlayVisible = false;
                }
            }
        }

        private async Task MarkAsPaidAsync()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Invoiced",
                "Do you want to mark this Customer Order as Paid?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    _isCustomerOrderFormOverlayVisible = true;
                    _isCustomerOrderItemFormOverlayVisible = true;
                    await CustomerOrdersService.MarkAsPaid(Id);
                    _customerOrder.Status = Core.Enums.OrderStatus.Paid;
                    Notify("Customer Order paid.", MudBlazor.Severity.Success);
                }
                catch (ValidationFailedException exc)
                {
                    var validationErrors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
                    var errorMessage = string.Join("<br/>", validationErrors);
                    Notify(errorMessage, MudBlazor.Severity.Error);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
                finally
                {
                    _isCustomerOrderFormOverlayVisible = false;
                    _isCustomerOrderItemFormOverlayVisible = false;
                }
            }
        }

        private async Task MarkAsCompletedAsync()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Invoiced",
                "Do you want to mark this Customer Order as Completed?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    _isCustomerOrderFormOverlayVisible = true;
                    _isCustomerOrderItemFormOverlayVisible = true;
                    await CustomerOrdersService.MarkAsCompleted(Id);
                    _customerOrder.Status = Core.Enums.OrderStatus.Completed;
                    Notify("Customer Order Completed.", MudBlazor.Severity.Success);
                }
                catch (ValidationFailedException exc)
                {
                    var validationErrors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
                    var errorMessage = string.Join("<br/>", validationErrors);
                    Notify(errorMessage, MudBlazor.Severity.Error);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
                finally
                {
                    _isCustomerOrderFormOverlayVisible = false;
                    _isCustomerOrderItemFormOverlayVisible = false;
                }
            }
        }

        private async Task PrintInvoiceAsync()
        {
            try
            {
                _isCustomerOrderFormOverlayVisible = true;
                _isCustomerOrderItemFormOverlayVisible = true;
                
                var file = await CustomerOrdersService.GeneratePdfInvoice(Id);

                //download pdf to client
                var fileStream = File.OpenRead(file);
                using var streamRef = new DotNetStreamReference(fileStream);
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", file, streamRef);
            }
            catch (Exception exc)
            {
                Notify(exc.Message, MudBlazor.Severity.Error);
            }
            finally
            {
                _isCustomerOrderFormOverlayVisible = false;
                _isCustomerOrderItemFormOverlayVisible = false;
            }

        }
    }

    public class CustomerOrderValidator : BaseValidator<Models.CustomerOrder>
    {
        public CustomerOrderValidator()
        {
            RuleFor(x => x.Customer).NotEmpty().WithMessage("Customer is required");
            RuleFor(x => x.OrderDate).NotEmpty().WithMessage("OrderDate is required");
        }
    }

    public class CustomerOrderItemValidator : BaseValidator<Models.CustomerOrderItem>
    {
        public CustomerOrderItemValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Required");
            RuleFor(x => x.Qty).NotEmpty().WithMessage("Required").GreaterThan(0).WithMessage("Invalid");
            RuleFor(x => x.UnitPrice).NotEmpty().WithMessage("Required").GreaterThan(0).WithMessage("Invalid");
        }
    }
}
