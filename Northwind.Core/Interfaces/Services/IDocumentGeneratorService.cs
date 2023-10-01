using Northwind.Core.Dtos.Document;

namespace Northwind.Core.Interfaces.Services
{
    public interface IDocumentGeneratorService
    {
        string CreateInvoicePdf(Invoice invoice);
        string CreatePurchaseOrderPdf(PurchaseOrderDto model);
    }
}