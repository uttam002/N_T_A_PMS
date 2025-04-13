using Azure;
using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class MenuService : IMenuService
{
    private readonly IItemService _itemService;
    private readonly IItemRepo _itemRepo;
    private readonly ICategoryRepo _categoryRepo;
    private readonly IModifierRepo _modifierRepo;

    private readonly IModifierService _modifierService;
    private readonly IItemModifierService _itemModifierService;

    public MenuService(IItemService itemService, ICategoryRepo categoryRepo, IItemRepo itemRepo, IModifierService modifierService, IItemModifierService itemModifierService)
    {
        _itemService = itemService;
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
        _modifierService = modifierService;
        _itemModifierService = itemModifierService;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> GetDefaultMenu(PaginationDetails paginationDetails)
    {
        try
        {
            List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
            List<ModifierGropDetails> modifierGroupList = await _modifierService.GetAllModifierGroups();
            List<ModifierDetails> modifiersForFirstGroup = await _modifierService.GetModifiersByModifierGroupId(modifierGroupList[0].id, paginationDetails);
            List<ItemDetails> itemsForFirstCategory = await _itemService.GetItemsByCategoryId(categories[0].CategoryId, paginationDetails);


            List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
            MenuDetails defaultMenu = new MenuDetails
            {
                categories = categoryList,
                items = itemsForFirstCategory,
                modifierGrops = modifierGroupList,
                modifiers = modifiersForFirstGroup
            };
            if (defaultMenu != null)
            {
                result.Message = "Sucessfully got Menu";
                result.Status = ResponseStatus.Success;
                result.Data = defaultMenu;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }


    #region  CRUD for Category
    public async Task<ResponseResult> AddCategory(CategoryDetails category)
    {
        try
        {
            Category existingCategory = await _categoryRepo.GetCategoryByNameAsync(category.categoryName);
            if (existingCategory != null)
            {
                result.Message = "Category already exists";
                result.Status = ResponseStatus.Error;
                return result;
            }
            Category newCategory = new Category
            {
                CategoryName = category.categoryName,
                Description = category.description,
                Createby = category.editorId,
                Createat = DateTime.Now,
                Isactive = true
            };

            result = await _categoryRepo.AddCategory(newCategory);
            if (result.Status == ResponseStatus.Success)
            {
                result.Message = "Category Added Successfully";
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                result.Data = categoryList;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> EditCategory(CategoryDetails category)
    {
        try
        {
            Category existingCategory = await _categoryRepo.GetCategoryByIdAsync(category.id);
            if (existingCategory == null)
            {
                result.Message = "Category not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            existingCategory.CategoryName = category.categoryName;
            existingCategory.Description = category.description;
            existingCategory.Modifyby = category.editorId;
            existingCategory.Modifyat = DateTime.Now;
            result = await _categoryRepo.UpdateCategory(existingCategory);
            if (result.Status == ResponseStatus.Success)
            {
                result.Message = "Category Updated Successfully";
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                result.Data = categoryList;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> DeleteCategory(int id, int editorId)
    {
        try
        {
            Category existingCategory = await _categoryRepo.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                result.Message = "Category not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            existingCategory.Isactive = false;
            existingCategory.Modifyby = editorId;
            existingCategory.Modifyat = DateTime.Now;
            result = await _categoryRepo.UpdateCategory(existingCategory);
            if (result.Status == ResponseStatus.Success)
            {
                await _itemRepo.DeleteAllItemsByCategoryIdAsync(id, editorId);
                result.Message = "Category Deleted Successfully";
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                result.Data = categoryList;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    #endregion


    #region CRUD for Item
    public async Task<ResponseResult> GetItems(int id, PaginationDetails paginationDetails)
    {
        try
        {
            List<ItemDetails> items = await _itemService.GetItemsByCategoryId(id, paginationDetails);
            if (items.Count > 0)
            {
                result.Message = "Sucessfully got Items";
                result.Status = ResponseStatus.Success;
                result.Data = items;
            }
            else
            {
                result.Message = "This Category don't any Items";
                result.Status = ResponseStatus.NotFound;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> AddItem(AddItem Item)
    {
        try
        {
            if (Item == null)
            {
                result.Message = "Failed to Load data into Model";
                result.Status = ResponseStatus.Error;
            }
            else
            {
                Item newItem = new Item();
                newItem.ItemName = Item.Name;
                newItem.Description = Item.Description;
                newItem.CategoryId = Item.CategoryId;
                newItem.ItemType = Item.ItemType;
                newItem.UnitType = Item.UnitType;
                newItem.UnitPrice = Item.UnitPrice;
                newItem.Quantity = Item.quantity;
                newItem.TaxPercentage = Item.TaxPercentage;
                newItem.IsAvailable = Item.IsAvailable;
                newItem.IsDefaultTax = Item.DefaultTax;
                newItem.ShortCode = Item.ShortCode;
                newItem.Createby = Item.EditorId;
                newItem.Createat = DateTime.Now;
                newItem.Isactive = true;
                if (Item.Photo != null)
                {
                    newItem.PhotoData = ConvertImageToByteArray(Item.Photo);
                }
                int itemId = await _itemRepo.AddItemAsync(newItem);
                if (itemId == 0)
                {
                    result.Message = "Failed to add item";
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                List<ItemModifierGroupRelation> itemModifiers = Item.IMDetails;
                if (itemModifiers != null)
                {
                    foreach (ItemModifierGroupRelation modifier in itemModifiers)
                    {
                        modifier.ItemId = itemId;
                    }
                    result = await _itemModifierService.AddMultipleEntriesOfItemModifier(itemModifiers);
                }
                else
                {
                    //No Modifiers are requierd to add in new Item
                    result.Status = ResponseStatus.Success;
                }
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = "Item Added Successfully";
                    PaginationDetails paginationDetails = new PaginationDetails();
                    List<ItemDetails> items = await _itemService.GetItemsByCategoryId(newItem.CategoryId, paginationDetails);
                    result.Data = (items, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> UpdateItem(AddItem UpdateItem)
    {
        try
        {
            Item existingItem = await _itemRepo.GetItemById(UpdateItem.itemID);
            if (existingItem == null)
            {
                result.Message = "Item not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            existingItem.ItemName = UpdateItem.Name;
            existingItem.Description = UpdateItem.Description;
            existingItem.CategoryId = UpdateItem.CategoryId;
            existingItem.ItemType = UpdateItem.ItemType;
            existingItem.UnitType = UpdateItem.UnitType;
            existingItem.UnitPrice = UpdateItem.UnitPrice;
            existingItem.Quantity = UpdateItem.quantity;
            existingItem.TaxPercentage = UpdateItem.TaxPercentage;
            existingItem.IsAvailable = UpdateItem.IsAvailable;
            existingItem.IsDefaultTax = UpdateItem.DefaultTax;
            existingItem.ShortCode = UpdateItem.ShortCode;
            existingItem.Modifyby = UpdateItem.EditorId;
            existingItem.Modifyat = DateTime.Now;
            existingItem.Isactive = true;
            if (UpdateItem.Photo != null)
            {
                existingItem.PhotoData = ConvertImageToByteArray(UpdateItem.Photo);
            }
            result = await _itemRepo.UpdateItemAsync(existingItem);

            if (result.Status == ResponseStatus.Success)
            {
                result = await _itemModifierService.UpdateItemModifiers(UpdateItem.IMDetails, UpdateItem.itemID);
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = "Item Updated Successfully";
                    PaginationDetails paginationDetails = new PaginationDetails();
                    result.Data = await _itemRepo.GetItemsByCategoryId(existingItem.CategoryId, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteItem(int itemId, int editorId)
    {
        try
        {
            Item existingItem = await _itemRepo.GetItemById(itemId);
            if (existingItem == null)
            {
                result.Message = "Item not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            existingItem.Isactive = false;
            existingItem.Modifyat = DateTime.Now;
            existingItem.Modifyby = editorId;
            result = await _itemRepo.UpdateItemAsync(existingItem);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> DeleteMultipleItems(int[] ids, int editorId)
    {
        try
        {
            List<Item> existingItems = await _itemRepo.GetItemListByIds(ids);
            if (existingItems == null || existingItems.Count == 0)
            {
                result.Message = "Items not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            foreach (Item item in existingItems)
            {
                item.Isactive = false;
                item.Modifyby = editorId;
                item.Modifyat = DateTime.Now;
            }
            return await _itemRepo.MassUpdateItemsAsync(existingItems);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return result;
        }
    }

    public async Task<AddItem> GetItemById(int itemId)
    {
        AddItem item = new AddItem();
        Item itemInfo = await _itemService.GetItemById(itemId);
        List<ItemModifierGroupRelation> itemModifierDetails = await _itemModifierService.GetItemModifierDetails(itemId);
        item.itemID = itemInfo.ItemId;
        item.Name = itemInfo.ItemName;
        item.Description = itemInfo.Description ?? "";
        item.CategoryId = itemInfo.CategoryId;
        item.ItemType = itemInfo.ItemType;
        item.UnitType = itemInfo.UnitType;
        item.UnitPrice = itemInfo.UnitPrice;
        item.quantity = itemInfo.Quantity;
        item.TaxPercentage = itemInfo.TaxPercentage ?? 0;
        item.IsAvailable = itemInfo.IsAvailable ?? false;
        item.DefaultTax = itemInfo.IsDefaultTax ?? false;
        item.ShortCode = itemInfo.ShortCode ?? "";
        item.IMDetails = itemModifierDetails;

        return item;
    }


    #endregion



    #region CRUD for Modifier


    public async Task<ResponseResult> GetModifiers(int id, PaginationDetails paginationDetails)
    {
        try
        {
            List<ModifierDetails> modifiers = await _modifierService.GetModifiersByModifierGroupId(id, paginationDetails);
            if (modifiers.Count > 0)
            {
                result.Message = "Sucessfully got Modifiers";
                result.Status = ResponseStatus.Success;
                result.Data = modifiers;
            }
            else
            {
                result.Message = "This ModifierGroup don't any Modifiers";
                result.Status = ResponseStatus.NotFound;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }



    #endregion

    #region CRUD for ModifierGroup

    
    public async Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup)
    {
        try
        {
            if (newGroup == null)
            {
                result.Message = "Failed to Load data into Model";
                result.Status = ResponseStatus.Error;
            }
            else
            {
                ModifiersGroup modifiersGroup = new ModifiersGroup
                {
                    MgName = newGroup.groupName,
                    Description = newGroup.description,
                    Createby = newGroup.editorId,
                    Createat = DateTime.Now,
                    Isactive = true
                };
                result = await _modifierRepo.AddModifierGroupAsync(modifiersGroup);
                if (result.Status == ResponseStatus.Success)
                {
                    result = await UpdateModifierMappingRelations(newGroup.groupName, 0, newGroup.modifierIds);
                    if (result.Status == ResponseStatus.Success)
                    {
                        result.Message = "Modifier Group Added Successfully";
                        List<ModifierGropDetails> modifierGroupList = await _modifierRepo.GetAllModifierGroups();
                        result.Data = modifierGroupList;
                    }
                    else
                    {
                        result.Message = "Failed to add Modifier Group Mappings";
                    }
                }
                else
                {
                    result.Message = "Failed to add Modifier Group";
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    private async Task<ResponseResult> UpdateModifierMappingRelations(string groupName, int groupId, List<int> modifierIds)
    {
        if (groupId == 0)
        {
            groupId = await _modifierRepo.GetModifierGroupIdByName(groupName);
        }
        if (groupId == 0)
        {
            result.Message = "Modifier Group not found";
            result.Status = ResponseStatus.Error;
            return result;
        }

        List<ModifierModifierGroupRelation> existingMappings = await _modifierRepo.GetModifiersRelationsByGroupid(groupId);
        List<ModifierModifierGroupRelation> newMappings = new List<ModifierModifierGroupRelation>();
        foreach (int modifierId in modifierIds)
        {
            newMappings.Add(new ModifierModifierGroupRelation
            {
                ModifierId = modifierId,
                GroupId = groupId
            });
        }

        List<ModifierModifierGroupRelation> addNewRelations = newMappings.Except(existingMappings).ToList();
        List<ModifierModifierGroupRelation> removeRelations = existingMappings.Except(newMappings).ToList();
        List<ModifierModifierGroupRelation> newGroupMapping = new List<ModifierModifierGroupRelation>();
        result = await _modifierRepo.AddNewModifierAndGroupRelationsAsync(addNewRelations);
        if (result.Status == ResponseStatus.Error)
        {
            result.Message = "Failed to add new Modifier-ModifierGroup relations";
            result.Status = ResponseStatus.Error;
            return result;
        }
        result = await _modifierRepo.RemoveModifierAndGroupRelationsAsync(removeRelations);
        if (result.Status == ResponseStatus.Error)
        {
            result.Message = "Failed to remove Modifier-ModifierGroup relations";
            result.Status = ResponseStatus.Error;
            return result;
        }
        else
        {
            result.Message = "Modifier-ModifierGroup relations Successfully updated!!!";
            result.Status = ResponseStatus.Success;
        }
        return result;
    }
    public async Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM updateGroup)
    {
        try
        {
            if (updateGroup == null)
            {
                result.Message = "Failed to Load data into Model";
                result.Status = ResponseStatus.Error;
            }
            else
            {
                ModifiersGroup existingGroup = await _modifierRepo.GetModifierGroupById(updateGroup.groupId);
                existingGroup.MgName = updateGroup.groupName;
                existingGroup.Description = updateGroup.description;
                existingGroup.Modifyby = updateGroup.editorId;
                existingGroup.Modifyat = DateTime.Now;
                result = await _modifierRepo.UpdateModifierGroupAsync(existingGroup);
                if (result.Status == ResponseStatus.Success)
                {
                    result = await UpdateModifierMappingRelations(updateGroup.groupName, updateGroup.groupId, updateGroup.modifierIds);
                    if (result.Status == ResponseStatus.Success)
                    {
                        result.Message = "Modifier Group Updated Successfully";
                        List<ModifierGropDetails> modifierGroupList = await _modifierRepo.GetAllModifierGroups();
                        result.Data = modifierGroupList;
                    }
                    else
                    {
                        result.Message = "Failed to update Modifier Group Mappings";
                    }
                }
                else
                {
                    result.Message = "Failed to update Modifier Group";
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteModifierGroup(int modifierGroupId, int editorId)
    {
        try
        {
            ModifiersGroup existingModifierGroup = await _modifierRepo.GetModifierGroupById(modifierGroupId);
            if (existingModifierGroup == null)
            {
                result.Message = "Modifier Group not found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            existingModifierGroup.Isactive = false;
            existingModifierGroup.Modifyby = editorId;
            existingModifierGroup.Modifyat = DateTime.Now;
            result = await _modifierRepo.UpdateModifierGroupAsync(existingModifierGroup);


            result = await _modifierService.DeleteModifierGroupByModifierGroupId(modifierGroupId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    #endregion
    #region CRUD for ItemModifier
    #endregion
    
    #region CRUD for ItemModifierGroup
    #endregion

    public Task<ResponseResult> DeleteMultipleModifiers(int[] ids)
    {
        return _modifierService.DeleteMultipleModifiers(ids);
    }

    public async Task<ResponseResult> AddNewModifier(AddModifier newModifer)
    {
        try
        {
            if (newModifer == null)
            {
                result.Message = "Failed to Load data into Model";
                result.Status = ResponseStatus.Error;
            }
            else
            {
                result = await _modifierService.AddNewModifier(newModifer);
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteModifier(int modifierId)
    {
        try
        {
            result = await _modifierService.DeleteModifierByModifierId(modifierId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetModifierByModifierId(int modifierId)
    {
        try
        {
            result = await _modifierService.GetModifierByModifierId(modifierId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> EditModifier(AddModifier editModifier)
    {
        try
        {
            result = await _modifierService.EditModifier(editModifier);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails)
    {
        return _modifierService.GetAllModifiers(paginationDetails);
    }
    public async Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId)
    {
        return await _modifierService.GetModifiersByGroupId(groupId);
    }



    //Helper methods
    public static byte[] ConvertImageToByteArray(IFormFile file)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
    public static IFormFile ConvertByteArrayToImage(byte[] imageData)
    {
        using (MemoryStream memoryStream = new MemoryStream(imageData))
        {
            return new FormFile(memoryStream, 0, imageData.Length, "file", "image.jpg");
        }
    }

    // Convert Entity Model to ViewModel
    private List<CategoryDetails> ConvertToCategoryList(List<Category> categories)
    {
        List<CategoryDetails> categoryList = categories.Select(category => new CategoryDetails
        {
            id = category.CategoryId,
            categoryName = category.CategoryName,
            description = category.Description
        }).ToList();
        return categoryList;
    }

    // private List<ItemDetails> ConvertToItemList(List<Item> items)
    // {
    //     List<ItemDetails> itemList = items.Select(item => new ItemDetails
    //     {
    //         id = item.ItemId,
    //         itemName = item.ItemName,
    //         itemType = item.ItemType,
    //         unitType = item.UnitType,
    //         unitPrice = item.UnitPrice,
    //         quantity = item.Quantity,
    //         isAvailable = item.IsAvailable ?? false,
    //         photo = ConvertByteArrayToImage(item.PhotoData)
    //     }).ToList();
    //     return itemList;
    // }
}
