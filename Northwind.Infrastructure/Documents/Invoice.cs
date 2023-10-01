using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentDtos = Northwind.Core.Dtos.Document;

namespace Northwind.Infrastructure.Documents
{
    public class Invoice : IDocument
    {
        private readonly DocumentDtos.Invoice _invoice;
        public Invoice(DocumentDtos.Invoice invoice)
        {
            _invoice = invoice;
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
                            column.Item().Text(_invoice.Company.Name).Bold();
                            column.Item().Text($"{_invoice.Company.Address.Street},");
                            column.Item().Text($"{_invoice.Company.Address.City}, {_invoice.Company.Address.State}, {_invoice.Company.Address.PostalCode}");
                            column.Item().Text(_invoice.Company.Address.Phone);
                        });
                        row.RelativeItem(4).Image(Placeholders.Image(100, 50));
                    });

                    //Invoice # & date & due date
                    column.Item().PaddingTop(10).Text("Invoice").FontSize(16).Bold();
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text($"Invoice #: {_invoice.InvoiceNumber}");
                        row.ConstantItem(100).Text($"Date: {_invoice.Date.ToString("MM/dd/yyyy")}");
                        row.ConstantItem(200).Text($"Due Date: {_invoice.DueDate.ToString("MM/dd/yyyy")}");
                    });

                    //supplier & ship to
                    column.Item().BorderBottom(2).BorderColor(Colors.Grey.Medium).PaddingBottom(10).PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem(6).Column(column =>
                        {
                            column.Item().Text("BILL To").Bold();
                            column.Item().Text(_invoice.BillTo.Name);
                            column.Item().Text($"{_invoice.BillTo.Address.Street},");
                            column.Item().Text($"{_invoice.BillTo.Address.City}, {_invoice.BillTo.Address.State}, {_invoice.BillTo.Address.PostalCode}");
                            column.Item().Text(_invoice.BillTo.Address.Phone);
                        });
                        row.RelativeItem(6).Column(column =>
                        {
                            column.Item().Text("SHIP TO").Bold();
                            column.Item().Text(_invoice.ShipTo.Name);
                            column.Item().Text($"{_invoice.ShipTo.Address.Street},");
                            column.Item().Text($"{_invoice.ShipTo.Address.City}, {_invoice.ShipTo.Address.State}, {_invoice.ShipTo.Address.PostalCode}");
                            column.Item().Text(_invoice.ShipTo.Address.Phone);
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

                        if (_invoice.Items != null)
                            foreach (var item in _invoice.Items)
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
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_invoice.Subtotal.ToString("N2"));
                        table.Cell().RowSpan(3).ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("Tax").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_invoice.Tax.ToString("N2"));
                        table.Cell().ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("Shipping").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_invoice.ShippingCost.ToString("N2"));
                        table.Cell().ColumnSpan(2).Border(1).Text("");
                        table.Cell().Row(rowIndex).Column(3).Border(1).PaddingHorizontal(5).AlignRight().Text("TOTAL").Bold();
                        table.Cell().Row(rowIndex++).Column(4).Border(1).PaddingHorizontal(5).AlignRight().Text(_invoice.Total.ToString("N2"));
                    });

                    column.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem(6).Text($"Prepared by: {_invoice.PreparedBy}");
                        row.RelativeItem(6).Text($"Approved by: {_invoice.ApprovedBy}");
                    });
                });
            });
        }
    }
}
