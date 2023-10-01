using Northwind.Core.Dtos.Document;
using Northwind.Core.Interfaces.Services;
using Northwind.Infrastructure.Documents;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Services
{
    public class DocumentGeneratorService : IDocumentGeneratorService
    {
        public string CreateInvoicePdf(Core.Dtos.Document.Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException("invoice");

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var filename = $"invoice-{invoice.InvoiceNumber}.pdf";
            var document = new Documents.Invoice(invoice);
            document.GeneratePdf(filename);

            return filename;
        }

        public string CreatePurchaseOrderPdf(PurchaseOrderDto model)
        {
            if (model == null) throw new ArgumentNullException("model");

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var filename = $"purchase-order-{model.PONumber}.pdf";
            var document = new PurchaseOrder(model);
            document.GeneratePdf(filename);

            return filename;
        }
    }
}
