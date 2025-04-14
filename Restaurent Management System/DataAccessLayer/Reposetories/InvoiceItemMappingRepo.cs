using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class InvoiceItemMappingRepo : IInvoiceItemMappingRepo
{
    private readonly AppDbContext _appDbContext;
    public InvoiceItemMappingRepo(AppDbContext appDbContext){
        _appDbContext = appDbContext;
    }
    ResponseResult result = new ResponseResult();
    public async Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId)
    {
        return await _appDbContext.InvoiceItemModifierMappings.Where(i=>i.OrderId == orderId).ToListAsync();
    }
    public async Task<List<InvoiceItemModifierMapping>> GetItemsForKOTAsync(int orderId)
    {
        return await _appDbContext.InvoiceItemModifierMappings.Where(i=>i.OrderId == orderId).ToListAsync();
    }

    public async Task<ResponseResult> UpdateItemMappingAsync(InvoiceItemModifierMapping invoiceItemModifierMapping)
    {
        try{
            _appDbContext.InvoiceItemModifierMappings.Update(invoiceItemModifierMapping);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Item Mapping Updated Successfully";
            result.Status = ResponseStatus.Success;
        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

}
