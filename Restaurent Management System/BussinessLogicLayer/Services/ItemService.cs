using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;

    public ItemService(IItemRepo itemRepo)
    {
        _itemRepo = itemRepo;
    }


    ResponseResult result = new ResponseResult();

    public async Task<int> AddItem(AddItem newItem)
    {
        return await _itemRepo.AddItem(newItem);
    }

    // public async Task<ResponseResult> DeleteCategoryByCategoryId(int id,int editorId)
    // {
    //     Category existingCategory = await _itemRepo.GetCategoryByIdAsync(id);
    //     if (existingCategory == null){
    //         result.Message = "Category Not Found";
    //         result.Status = ResponseStatus.Error;
    //         return result;
    //     }
    //     //Soft Delete
    //     existingCategory.Isactive = false;
    //     existingCategory.Modifyby = editorId;
    //     existingCategory.Modifyat = DateTime.Now;
    //     result = await _itemRepo.UpdateCategory(existingCategory);
        
    //     if (result.Status == ResponseStatus.Success)
    //     {
    //         result.Message = "Category Deleted Successfully";
    //     }
    //     return result;
    // }

    public async Task<ResponseResult> DeleteItemByItemId(int itemId)
    {

        return await _itemRepo.DeleteItemByItemIdAsync(itemId);
    }

    public async Task<ResponseResult> EditItem(AddItem updateItem)
    {
        if (updateItem.itemID == 0)
        {
            result.Message = "Item Id is not Valid";
            result.Status = ResponseStatus.Error;
            return result;
        }
        return await _itemRepo.UpdateItemAsync(updateItem);
    }
    public async Task<List<ItemDetails>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails)
    {
        List<Item> itemList = await _itemRepo.GetItemsByCategoryId(id, paginationDetails);
        List<ItemDetails> itemDetailsList = new List<ItemDetails>();
        foreach (var item in itemList){
            ItemDetails itemDetails = new ItemDetails();
            itemDetails.id = item.ItemId;
            itemDetails.itemName = item.ItemName;
            itemDetails.itemType = item.ItemType;
            itemDetails.unitPrice = item.UnitPrice;
            itemDetails.unitType = item.UnitType;
            itemDetails.quantity = item.Quantity;
            itemDetails.isAvailable = item.IsAvailable??false;
            // itemDetails.photo = ConvertByteArrayToImage(item.PhotoData,item.ItemId);
            itemDetails.photo = ConvertToBase64Image(item.PhotoData);
            itemDetailsList.Add(itemDetails);
        }
        return itemDetailsList;
    }
    public async Task<Item> GetItemById(int itemId)
    {
        return await _itemRepo.GetItemById(itemId);
    }
      public static IFormFile? ConvertByteArrayToImage(byte[] imageData,int itemId)
    {
        if (imageData == null || imageData.Length == 0)
        {
            return null; // or handle the case when there's no image data
        }
        string fileName = $"image_{itemId}.jpg"; // Generate a unique file name
        using (MemoryStream memoryStream = new MemoryStream(imageData))
        {
            return new FormFile(memoryStream, 0, imageData.Length, "file", fileName);
        }
    }
    public static string? ConvertToBase64Image(byte[] imageData)
{
    if (imageData == null || imageData.Length == 0)
        return null;

    string base64String = Convert.ToBase64String(imageData);
    return $"data:image/jpeg;base64,{base64String}";
}
}
