using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;

namespace PMSServices.Interfaces;

public interface IItemService
{
    Task<int> AddItem(AddItem newItem);
    Task<ResponseResult> EditItem(AddItem updateItem);
    // Task<ResponseResult> DeleteCategoryByCategoryId(int id);
    // Task<ResponseResult> DeleteCategoryByItemId(object itemId);
    Task<ResponseResult> DeleteItemByItemId(int itemId);
    // Task<List<CategoryDetails>> GetAllCategories();
    Task<List<ItemDetails>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails);

    Task<Item> GetItemById(int itemId);
}
