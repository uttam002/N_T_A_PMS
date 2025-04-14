using PMSCore.Beans;

namespace PMSData.Interfaces;

public interface IInvoiceItemMappingRepo
{
    Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId);
    Task<List<InvoiceItemModifierMapping>> GetItemsForKOTAsync(int orderId);
    Task<ResponseResult> UpdateItemMappingAsync(InvoiceItemModifierMapping invoiceItemModifierMapping);

}
