using Microsoft.EntityFrameworkCore;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class InvoiceItemMappingRepo : IInvoiceItemMappingRepo
{
    private readonly AppDbContext _appDbContext;
    public InvoiceItemMappingRepo(AppDbContext appDbContext){
        _appDbContext = appDbContext;
    }
    public async Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId)
    {
        return await _appDbContext.InvoiceItemModifierMappings.Where(i=>i.OrderId == orderId).ToListAsync();
    }

}
