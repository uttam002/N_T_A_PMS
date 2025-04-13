using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IItemRepo
{
   
    Task<ResponseResult> MassUpdateItemsAsync(List<Item> items);
    Task<int> AddItem(AddItem newItem);
    Task<ResponseResult> DeleteItemByItemIdAsync(int itemId);
    Task DeleteAllItemsByCategoryIdAsync(int categoryId,int editorId);
    Task<List<Item>> GetItemListByIds(int[] itemIds);
    Task<Item> GetItemById(int itemId);
    Task<string> GetItemNameByIdAsync(int itemId);
    Task<List<Item>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails);
    Task<ResponseResult> UpdateItemAsync(Item updateItem);
    Task<ResponseResult> UpdateItemAsync(AddItem updateItem);
    Task<List<string>> GetItemNamesByIdsAsync(int[] itemIds);
    Task<int> AddItemAsync(Item newItem);
}
