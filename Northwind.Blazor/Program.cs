using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Northwind.Blazor.Data;
using Northwind.Common.Validators;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using Northwind.Core.Services;
using Northwind.Data;
using Northwind.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<EfDbContext>();
builder.Services.AddSingleton<IProductsService, ProductsService>();
builder.Services.AddSingleton<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddSingleton<IProductValidator, ProductValidator>();

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
