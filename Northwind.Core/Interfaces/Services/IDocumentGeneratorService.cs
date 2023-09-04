using Northwind.Core.Dtos.Document;

namespace Northwind.Core.Interfaces.Services
{
    public interface IDocumentGeneratorService
    {
        string CreatePurchaseOrderPdf(PurchaseOrderDto model);
    }
}