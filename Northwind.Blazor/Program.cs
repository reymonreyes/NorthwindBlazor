using MudBlazor.Services;
using Northwind.Blazor.Data;
using Northwind.Common.Validators;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using Northwind.Core.Services;
using Northwind.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<ICategoriesService, CategoriesService>();
builder.Services.AddTransient<ISuppliersService, SuppliersService>();
builder.Services.AddTransient<IShippersService, ShippersService>();
builder.Services.AddTransient<ICustomersService, CustomersService>();
builder.Services.AddTransient<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddSingleton<IProductValidator, ProductValidator>();
builder.Services.AddSingleton<ICategoryValidator, CategoryValidator>();
builder.Services.AddSingleton<ISupplierValidator, SupplierValidator>();
builder.Services.AddSingleton<IShipperValidator, ShipperValidator>();
builder.Services.AddSingleton<ICustomerValidator, CustomerValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
