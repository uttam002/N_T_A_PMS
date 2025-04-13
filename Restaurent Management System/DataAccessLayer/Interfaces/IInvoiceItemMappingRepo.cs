namespace PMSData.Interfaces;

public interface IInvoiceItemMappingRepo
{
    Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId);
}
