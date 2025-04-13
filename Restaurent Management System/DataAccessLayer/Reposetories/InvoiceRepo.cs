using Microsoft.EntityFrameworkCore;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class InvoiceRepo : IInvoiceRepo
{
    private readonly AppDbContext _appDbContext;
    public InvoiceRepo(AppDbContext appDbContext){
        _appDbContext = appDbContext;
    }

    public async Task<Invoice> GetInvoiceNumberAsync(int orderId)
    {
            return await _appDbContext.Invoices.Where(i=>i.OrderId == orderId).FirstAsync();
    } 

}
