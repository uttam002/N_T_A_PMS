namespace PMSData.Interfaces;

public interface IInvoiceRepo
{
    Task<Invoice> GetInvoiceNumberAsync(int orderId);
}
