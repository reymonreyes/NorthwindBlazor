using Northwind.Core.Dtos.Document;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Documents
{
    public class PurchaseOrder : IDocument
    {
        private readonly PurchaseOrderDto _model;
        public PurchaseOrder(PurchaseOrderDto model)
        {
            _model = model;
        }
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // page content
                page.MarginTop(1.91f, Unit.Centimetre);
                page.MarginBottom(1.91f, Unit.Centimetre);
                page.MarginLeft(1.78f, Unit.Centimetre);
                page.MarginRight(1.78f, Unit.Centimetre);

                page.Content().Column(column =>
                {
                    //company address & logo
                    column.Item().BorderBottom(2).BorderColor(Colors.Grey.Medium).PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem(8).Column(column =>
                        {
                            column.Item().Text(_model?.Company?.Name).Bold();
                            column.Item().Text($"{_model?.Company?.Address?.Street},");
                            column.Item().Text($"{_model?.Company?.Address?.City}, {_model?.Company?.Address?.State}, {_model?.Company?.Address?.PostalCode}");
                            column.Item().Text(_model?.Company?.Address?.Phone);
                        });
                        row.RelativeItem(4).Image(Placeholders.Image(100, 50));
                    });

                    //PO # & date
                    column.Item().PaddingTop(10).Text("PURCHASE ORDER").FontSize(16).Bold();
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text($"PO#: {_model?.PONumber}");
                        row.ConstantItem(100).Text($"Date: {_model?.Date.ToString("MM/dd/yyyy")}");
                    });

                    //supplier & ship to
                    column.Item().BorderBottom(2).BorderColor(Colors.Grey.Medium).PaddingBottom(10).PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem(6).Column(column =>
                        {
                            column.Item().Text("SUPPLIER").Bold();
                            column.Item().Text(_model?.Supplier?.Name);
                            column.Item().Text($"{_model?.Supplier?.Address?.Street},");
                            column.Item().Text($"{_model?.Supplier?.Address?.City}, {_model?.Supplier?.Address?.State}, {_model?.Supplier?.Address?.PostalCode}");
                            column.Item().Text(_model?.Supplier?.Address?.Phone);
                        });
                        row.RelativeItem(6).Column(column =>
                        {
                            column.Item().Text("SHIP TO").Bold();
                            column.Item().Text(_model.ShipTo.Name);
                            column.Item().Text($"{_model?.ShipTo?.Address?.Street},");
                            column.Item().Text($"{_model?.ShipTo?.Address?.City}, {_model?.ShipTo?.Address?.State}, {_model?.ShipTo?.Address?.PostalCode}");
                            column.Item().Text(_model?.ShipTo?.Address?.Phone);
                        });
                    });

                    //items table
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        //table headers
                        table.Cell().Row(1).Column(1).Border(1).AlignCenter().Text("Item Description").Bold();
                        table.Cell().Row(1).Column(2).Border(1).AlignCenter().Text("Qty").Bold();
                        table.Cell().Row(1).Column(3).Border(1).AlignCenter().Text("Unit Price").Bold();
                        table.Cell().Row(1).Column(4).Border(1).AlignCenter().Text("Total").Bold();

                        //items
                        uint rowIndex = 2;
                        if (_model.LineItems != null)
                            foreach (var item in _model.LineItems)
                            {
                                table.Cell().Row(rowIndex).Column(1).Border(1).PaddingHorizontal(5).AlignLeft().Text(item.ItemDescription);
                                table.Cell().Row(rowIndex).Column(2).Border(1).PaddingHorizontal(5).AlignRight().Text(item.Qty.ToString());
                                table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text(item.UnitPrice.ToString("N2"));
                                table.Cell().Row(rowIndex).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(item.Total.ToString("N2"));
                                rowIndex++;
                            }

                        //totals
                        table.Cell().ColumnSpan(4).BorderHorizontal(1).Text("");
                        rowIndex++;
                        table.Cell().ColumnSpan(2).Border(1).PaddingLeft(5).Text("Notes").Bold();
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("Subtotal").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_model?.Subtotal.ToString("N2"));
                        table.Cell().RowSpan(3).ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("Tax").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_model?.Tax.ToString("N2"));
                        table.Cell().ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("Shipping").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_model?.ShippingCost.ToString("N2"));
                        table.Cell().ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("TOTAL").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_model?.Total.ToString("N2"));
                    });

                    column.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem(6).Text($"Prepared by: {_model?.PreparedBy}");
                        row.RelativeItem(6).Text($"Approved by: {_model?.ApprovedBy}");
                    });
                });
            });
        }
    }
}
