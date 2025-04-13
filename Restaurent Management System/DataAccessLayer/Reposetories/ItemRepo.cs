using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class ItemRepo : IItemRepo
{
    private readonly AppDbContext _appDbContext;

    public ItemRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();


    public async Task<List<Item>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails)
    {
        var query = _appDbContext.Items.Where(a => a.CategoryId == id && a.Isactive == true);
        if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
        {
            query = query.Where(a => a.ItemName.Contains(paginationDetails.SearchQuery));
        }
        paginationDetails.totalRecords = await query.CountAsync(); // Count filtered results
        return await query
                    .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                    .Take(paginationDetails.PageSize)
                    .ToListAsync();

    }

    public async Task<int> AddItemAsync(Item newItem)
    {
        try{
            _appDbContext.Items.Add(newItem);
            await _appDbContext.SaveChangesAsync();
            return await GetItemIdByItemName(newItem.ItemName);
        }catch{
            return 0;
        }
    }
    
    public async Task<ResponseResult> UpdateItemAsync(Item updateItem)
    {
        try
        {
            _appDbContext.Items.Update(updateItem);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Successfully Updated Item";
            result.Status = ResponseStatus.Success;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> MassUpdateItemsAsync(List<Item> items)
    {
        try
        {
            foreach (Item item in items)
            {
                // Item item = await GetItemById(id);
                // item.Isactive = false;
                // item.Modifyat = DateTime.Now;
                _appDbContext.Items.Update(item);
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Mass Update Items Successfully!!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        return result;

    }


    
    
    
    
    public async Task<ResponseResult> DeleteItemByItemIdAsync(int itemId)
    {
        var item = await _appDbContext.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item == null)
        {
            result.Message = "Item is not Found";
            result.Status = ResponseStatus.NotFound;
            return result;
        }
        item.Isactive = false;
        await _appDbContext.SaveChangesAsync();
        result.Message = "Item is successfully deleted";
        result.Status = ResponseStatus.Success;
        return result;


    }



    public async Task DeleteAllItemsByCategoryIdAsync(int categoryId, int editorId)
    {
        List<Item> items = await _appDbContext.Items.Where(i => i.CategoryId == categoryId).ToListAsync();
        foreach (Item item in items)
        {
            item.Isactive = false;
            item.Modifyby = editorId;
            item.Modifyat = DateTime.Now;
            _appDbContext.Items.Update(item);
        }
        await _appDbContext.SaveChangesAsync();
    }



    
    public async Task<int> GetItemIdByItemName(string itemName)
    {

        int itemId = await _appDbContext.Items.Where(x => x.ItemName == itemName).Select(i => i.ItemId).FirstOrDefaultAsync();
        return itemId;
    }
    public async Task<string> GetItemNameByIdAsync(int itemId)
    {

        string? itemName = await _appDbContext.Items.Where(x => x.ItemId == itemId).Select(i => i.ItemName).FirstOrDefaultAsync();
        return itemName ?? "";
    }

    public async Task<Item> GetItemById(int itemId)
    {
        Item item = await _appDbContext.Items.Where(x => x.ItemId == itemId && x.Isactive == true).FirstOrDefaultAsync();
        return item;
    }
    public async Task<List<Item>> GetItemListByIds(int[] itemIds){
        List<Item> items = await _appDbContext.Items.Where(x => itemIds.Contains(x.ItemId) && x.Isactive == true).ToListAsync();
        return items;
    }
    public Task<List<string>> GetItemNamesByIdsAsync(int[] itemIds)
    {
        throw new NotImplementedException();
    }

    public Task<int> AddItem(AddItem newItem)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseResult> UpdateItemAsync(AddItem updateItem)
    {
        throw new NotImplementedException();
    }
      public static IFormFile ConvertByteArrayToImage(byte[] imageData)
    {
        using (MemoryStream memoryStream = new MemoryStream(imageData))
        {
            return new FormFile(memoryStream, 0, imageData.Length, "file", "image.jpg");
        }
    }
}

