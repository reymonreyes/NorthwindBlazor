using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Northwind.Blazor.Helpers;
using Northwind.Blazor.Models;
using Northwind.Common.Enums;
using Northwind.Core.Dtos;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Services;
using System;

namespace Northwind.Blazor.Pages.purchaseorders
{
    public partial class Details
    {
        private EntryMode _entryMode = EntryMode.Create;
        public bool _isPODetailsValid = false, _isAddItemFormValid;
        private MudForm _poForm, _addItemForm;
        private PurchaseOrder _purchaseOrder = new PurchaseOrder();
        private PurchaseOrderItem _purchaseOrderItem = new PurchaseOrderItem();
        private string _supplierSearch;
        private PurchaseOrderFluentValidator _purchaseOrderValidator = new PurchaseOrderFluentValidator();
        private PurchaseOrderItemFluentValidator _purchaseOrderItemValidator = new PurchaseOrderItemFluentValidator();
        private bool _isOverlayVisible = false;
        private MudAutocomplete<ProductDto> _productAutocomplete;


        private List<string> _errors = new List<string>();
        string[] headings = { "ID", "Description", "Qty", "Unit price", "Total" };

        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISuppliersService SuppliersService { get; set; }
        [Inject]
        public IPurchaseOrdersService PurchaseOrdersService { get; set; }
        [Inject]
        public IProductsService ProductsService { get; set; }
        [Inject]
        public IDialogService DialogService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitialized--");

            _entryMode = NavigationManager.Uri.EndsWith("create") ? EntryMode.Create : EntryMode.Edit;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    _purchaseOrder = new PurchaseOrder();
                    _purchaseOrder.Items = new List<PurchaseOrderItem>();
                    _purchaseOrder.OrderDate = DateTime.Now;
                    break;
                case EntryMode.Edit:
                    await LoadPurchaseOrder();
                    break;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("OnAfterRenderAsync()");
            await _poForm.Validate();
        }

        private async Task LoadPurchaseOrder()
        {
            Console.WriteLine("LoadPurchaseOrder()");
            var purchaseOrder = await PurchaseOrdersService.GetAsync(Id.Value);
            if (purchaseOrder is not null)
            {
                _purchaseOrder = new PurchaseOrder
                {
                    ShipTo = purchaseOrder.ShipTo,
                    OrderDate = purchaseOrder.OrderDate.ToLocalTime(),
                    Notes = purchaseOrder.Notes,
                    Status = purchaseOrder.Status,
                    Items = purchaseOrder.OrderItems.Select(x => new PurchaseOrderItem { Id = x.Id, Product = new ProductDto { Id = x.Id }, Quantity = x.Quantity, UnitPrice = x.UnitPrice.Value }).ToList()
                };

                var supplier = await SuppliersService.Get(purchaseOrder.SupplierId);
                if (supplier is not null)
                    _purchaseOrder.Supplier = new SupplierDto { Id = supplier.Id, Name = supplier.Name };

                Console.WriteLine("PurchaseOrder loaded");
            }
        }

        private async Task<IEnumerable<SupplierDto>> FindSupplier(string supplierName)
        {
            if (string.IsNullOrWhiteSpace(supplierName))
                return new List<SupplierDto>();

            Console.WriteLine("FindSupplier");

            ICollection<SupplierDto> matchedSuppliers = await SuppliersService.Find(supplierName);

            return matchedSuppliers;
        }

        private async Task<IEnumerable<ProductDto>> FindProduct(string productName)
        {
            Console.WriteLine("FindProduct");
            if (string.IsNullOrWhiteSpace(productName))
                return new List<ProductDto>();

            ICollection<ProductDto> matchedProducts = await ProductsService.Find(productName);

            return matchedProducts;
        }

        private async Task<bool> Save()
        {
            Console.WriteLine("Save()");
            await _poForm.Validate();

            if (!_isPODetailsValid)
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
            try
            {
                var purchaseOrder = _purchaseOrder.ToPurchaseOrderDto();
                var result = await PurchaseOrdersService.Create(purchaseOrder);
                if (result.IsSuccessful)
                {
                    var id = result?.Messages?.FirstOrDefault()?.Message.Value;
                    Notify($"Purchase Order # {id} {(_entryMode == EntryMode.Create ? "created" : "updated")} successfully.", MudBlazor.Severity.Success);
                    Id = int.Parse(id);
                    _entryMode = EntryMode.Edit;
                    NavigationManager.NavigateTo($"purchaseorders/{id}", false, true);
                }
            }
            catch (ValidationFailedException exc)//server side error
            {
                Console.WriteLine("creation failed");
                _errors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
            }
        }

        private async Task Update()
        {
            try
            {
                if (Id.HasValue)
                {
                    var purchaseOrder = _purchaseOrder.ToPurchaseOrderDto();
                    var result = await PurchaseOrdersService.UpdateAsync(Id.Value, purchaseOrder);
                    if (result.IsSuccessful)
                    {
                        Notify($"Purchase Order # {Id} {(_entryMode == EntryMode.Create ? "created" : "updated")} successfully.", MudBlazor.Severity.Success);
                    }
                }
            }
            catch (ValidationFailedException exc)//server side error
            {
                //TODO: logging
                _errors = exc.ValidationErrors.Select(x => x.Message.Value).ToList();
            }
        }

        private void OnProductValueChanged(ProductDto? productDto)
        {
            Console.WriteLine("ValueChanged");
            _purchaseOrderItem.Product = productDto;
            if (productDto is not null)
                _purchaseOrderItem.UnitPrice = productDto.ListPrice;
        }

        private async Task AddItem()
        {
            await _addItemForm.Validate();

            if (!_isAddItemFormValid)
                return;

            switch (_entryMode)
            {
                case EntryMode.Create:
                    //saved PO first before saving the item
                    var success = await Save();
                    if (success)
                        await AddItemToPurchaseOrder();
                    break;
                case EntryMode.Edit:
                    await AddItemToPurchaseOrder();
                    break;
            }
        }

        private async Task AddItemToPurchaseOrder()
        {
            _isOverlayVisible = true;

            var item = new PurchaseOrderItemDto
            {
                ProductId = _purchaseOrderItem.Product.Id,
                Quantity = _purchaseOrderItem.Quantity,
                UnitPrice = _purchaseOrderItem.UnitPrice
            };

            try
            {
                await PurchaseOrdersService.AddItem(Id.Value, item);
                _purchaseOrder.Items.Add(_purchaseOrderItem);
                _purchaseOrderItem = new PurchaseOrderItem();
                await _productAutocomplete.Clear();
                _isOverlayVisible = false;
                Notify($"Item added successfully.", MudBlazor.Severity.Success);
            }
            catch (Exception exc)
            {
                var message = exc.Message;
                Notify(message, MudBlazor.Severity.Error);
            }

            _isOverlayVisible = false;
        }

        private async Task RemoveItem(PurchaseOrderItem purchaseOrderItem)
        {
            _isOverlayVisible = true;
            _purchaseOrder.Items.Remove(purchaseOrderItem);
            await PurchaseOrdersService.RemoveItem(purchaseOrderItem.Id);
            _isOverlayVisible = false;
        }

        public void EditItem(PurchaseOrderItem purchaseOrderItem)
        {
            purchaseOrderItem.IsInEditMode = true;
        }

        private async Task UpdateItem(PurchaseOrderItem purchaseOrderItem)
        {
            Console.WriteLine("UpdateItem");
            _isOverlayVisible = true;

            var item = new PurchaseOrderItemDto
            {
                Id = purchaseOrderItem.Id,
                ProductId = purchaseOrderItem.Product.Id,
                Quantity = purchaseOrderItem.Quantity,
                UnitPrice = purchaseOrderItem.UnitPrice
            };

            try
            {
                await PurchaseOrdersService.UpdateItem(Id.Value, item);//add updateitemasync method
                purchaseOrderItem.IsInEditMode = false;
                purchaseOrderItem.EditItem = null;
                _isOverlayVisible = false;
                Notify($"Item added successfully.", MudBlazor.Severity.Success);
            }
            catch (Exception exc)
            {
                var message = exc.Message;
                Notify(message, MudBlazor.Severity.Error);
            }

            _isOverlayVisible = false;

            purchaseOrderItem.IsInEditMode = false;
        }

        private void CancelUpdateItem(PurchaseOrderItem purchaseOrderItem)
        {
            Console.WriteLine("CancelUpdateItem from parent");
            purchaseOrderItem.IsInEditMode = false;
            purchaseOrderItem.EditItem = null;
            StateHasChanged();
        }

        private async Task SubmitForApproval()
        {
            //prompt user for confirmation
            var confirmed = await DialogService.ShowMessageBox(
                "Submit for Approval",
                "Do you want to submit this Purchase Order for approval?",
                "Yes",
                "No"
            );
            Console.WriteLine($"confirmed: {confirmed}");
            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    var result = await PurchaseOrdersService.SubmitAsync(Id.Value);
                    _purchaseOrder.Status = Core.Enums.OrderStatus.Submitted;
                    Notify("Purchase Order successfully submitted.", MudBlazor.Severity.Success);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
            }
        }

        private async Task Approve()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Approve",
                "Do you want to approve this Purchase Order?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    var result = await PurchaseOrdersService.ApproveAsync(Id.Value);
                    _purchaseOrder.Status = Core.Enums.OrderStatus.Approved;
                    Notify("Purchase Order successfully submitted.", MudBlazor.Severity.Success);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
            }
        }

        private async Task Complete()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Close",
                "Do you want to complete this Purchase Order?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    await PurchaseOrdersService.CompleteOrder(Id.Value);
                    _purchaseOrder.Status = Core.Enums.OrderStatus.Completed;
                    Notify("Purchase Order successfully submitted.", MudBlazor.Severity.Success);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
            }
        }

        private async Task Cancel()
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Cancel",
                "Are you sure you want to cancel this Purchase Order?",
                "Yes",
                "No"
            );

            if (confirmed.HasValue && confirmed.Value)
            {
                try
                {
                    await PurchaseOrdersService.CancelAsync(Id.Value);
                    _purchaseOrder.Status = Core.Enums.OrderStatus.Cancelled;
                    Notify("Purchase Order cancelled.", MudBlazor.Severity.Warning);
                }
                catch (Exception exc)
                {
                    Notify(exc.Message, MudBlazor.Severity.Error);
                }
            }
        }

        private async Task PrintPOAsync()
        {
            //Notify("PrintPO", MudBlazor.Severity.Info);
            _isOverlayVisible = true;
            try
            {
                var file = await PurchaseOrdersService.GeneratePdfDocument(Id.Value);

                //download pdf to client
                var fileStream = File.OpenRead(file);
                using var streamRef = new DotNetStreamReference(fileStream);

                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", file, streamRef);
            }
            catch(Exception exc)
            {
                Notify(exc.Message, MudBlazor.Severity.Error);
            }
            finally
            {
                _isOverlayVisible = false;
            }
        }
    }
    public class PurchaseOrderFluentValidator : AbstractValidator<PurchaseOrder>
    {
        public PurchaseOrderFluentValidator()
        {
            RuleFor(x => x.OrderDate).NotEmpty().WithMessage("OrderDate is required");
            RuleFor(x => x.ShipTo).NotEmpty().WithMessage("ShipTo is required");
            RuleFor(X => X.Supplier).NotEmpty().WithMessage("Supplier is required");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PurchaseOrder>.CreateWithOptions((PurchaseOrder)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    public class PurchaseOrderItemFluentValidator : AbstractValidator<PurchaseOrderItem>
    {
        public PurchaseOrderItemFluentValidator()
        {
            RuleFor(x => x.Product).NotEmpty().WithMessage("Required");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Required").GreaterThanOrEqualTo(0).WithMessage("Invalid");
            RuleFor(x => x.UnitPrice).NotEmpty().WithMessage("Required").GreaterThanOrEqualTo(0).WithMessage("Invalid");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PurchaseOrderItem>.CreateWithOptions((PurchaseOrderItem)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
