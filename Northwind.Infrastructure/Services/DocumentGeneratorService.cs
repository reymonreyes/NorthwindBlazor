using Northwind.Core.Dtos.Document;
using Northwind.Infrastructure.Documents;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Services
{
    public class DocumentGeneratorService
    {
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
