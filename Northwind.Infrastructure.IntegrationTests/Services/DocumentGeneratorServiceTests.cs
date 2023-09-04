﻿using Autofac.Extras.Moq;
using Northwind.Core.Dtos;
using DocumentDtos = Northwind.Core.Dtos.Document;
using Northwind.Infrastructure.Services;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Core.Dtos.Document;
using UglyToad.PdfPig;
using Autofac.Builder;

namespace Northwind.Infrastructure.IntegrationTests.Services
{
    public class DocumentGeneratorServiceTests : IClassFixture<DocumentGeneratorServiceFixture>
    {
        private DocumentGeneratorServiceFixture _fixture;
        public DocumentGeneratorServiceTests(DocumentGeneratorServiceFixture fixture)
        {
            _fixture = fixture; 
        }

        [Fact]
        public void CreatePurchaseOrderPdf_ShouldCreatePdfOnCompleteData()
        {
            var mock = AutoMock.GetLoose();
            var service = mock.Create<DocumentGeneratorService>();
            _fixture.Model.LineItems = null;
            string filename = service.CreatePurchaseOrderPdf(_fixture.Model);
            //expect filename is in correct format
            Assert.Contains(_fixture.Filename, filename);

            //expect that content from inside of pdf matches the input data
            var pdfContent = _fixture.ReadPdfFile(filename);

            //just a simple check for company info, it would be too exhaustive if we check for each of the data in the pdf
            Assert.Contains(_fixture.Model.Company.Name, pdfContent);
        }

        [Fact]
        public void CreatePurchaseOrderPdf_ShouldThrowExceptionIfParameterIsNull()
        {
            var mock = AutoMock.GetLoose();
            var service = mock.Create<DocumentGeneratorService>();

            Assert.Throws<ArgumentNullException>(() => service.CreatePurchaseOrderPdf(null));
        }        
    }

    public class DocumentGeneratorServiceFixture
    {
        public DocumentGeneratorServiceFixture()
        {
            var model = new DocumentDtos.PurchaseOrderDto();
            model.Company = new BasicCompanyInformation { Name = "Northwind Traders", Address = new Address { Street = "123 First St.", City = "Scranton", State = "PA", PostalCode = "10001", Phone = "(999)123-4567" } };
            model.Supplier = new Supplier { Name = "Acme Supplier", Address = new Address { Street = "22nd St.", City = "New York", State = "NY", PostalCode = "15001", Phone = "(123) 456-7890" } };
            model.ShipTo = new ShipTo { Name = "Northwind Traders", Address = new Address { Street = "123 First St.", City = "Scranton", State = "PA", PostalCode = "10001", Phone = "(999)123-4567" } };
            model.PONumber = "12345";
            model.Date = DateTime.Now;
            model.Subtotal = 1234;
            model.Tax = 23.00m;
            model.ShippingCost = 8.74m;
            model.Total = 1500m;
            model.PreparedBy = "Alfred Doe";
            model.ApprovedBy = "Bruce Wayne";
            model.LineItems = new List<LineItem>();

            for (int i = 0; i < 5; i++)
            {
                model.LineItems.Add(new LineItem
                {
                    ItemDescription = Placeholders.Name(),
                    Qty = int.Parse(Placeholders.Integer()),
                    UnitPrice = decimal.Parse(Placeholders.Decimal()) * 100,
                    Total = decimal.Parse(Placeholders.Decimal()) * 100
                });
            }

            Model = model;
            Filename = $"purchase-order-{model.PONumber}";            
        }

        public DocumentDtos.PurchaseOrderDto Model { get; private set; }
        public string Filename { get; private set; }

        public string ReadPdfFile(string filename)
        {
            var pageText = string.Empty;
            using (PdfDocument document = PdfDocument.Open(filename))
            {
                foreach (var page in document.GetPages())
                {
                    pageText += page.Text;
                }
            }

            return pageText;
        }
    }

}
