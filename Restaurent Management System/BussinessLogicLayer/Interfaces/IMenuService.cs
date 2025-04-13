using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces;

public interface IMenuService
{
    Task<ResponseResult> GetDefaultMenu(PaginationDetails paginationDetails);

#region Category
    Task<ResponseResult> AddCategory(CategoryDetails newCategory);
    Task<ResponseResult> EditCategory(CategoryDetails updateCategory);
    Task<ResponseResult> DeleteCategory(int categoryId,int editorId);
#endregion
    Task<ResponseResult> AddItem(AddItem newItem);
    Task<ResponseResult> UpdateItem(AddItem UpdateItem);
    Task<ResponseResult> DeleteItem(int itemId,int editorId);
    Task<ResponseResult> GetItems(int id,PaginationDetails paginationDetails);
    Task<ResponseResult> GetModifiers(int id,PaginationDetails paginationDetails);
    Task<ResponseResult> DeleteMultipleItems(int[] ids,int editorId);
    Task<ResponseResult> AddNewModifier(AddModifier newModifer);
    Task<ResponseResult> DeleteModifierGroup(int modifierGroupId,int editorId);
    Task<ResponseResult> GetModifierByModifierId(int modifierId);
    Task<ResponseResult> DeleteModifier(int itemId);
    Task<ResponseResult> EditModifier(AddModifier editModifier);
    Task<AddItem> GetItemById(int itemId);
    Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails);
    Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup);
    Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM updateGroup);
    Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId);
    Task<ResponseResult> DeleteMultipleModifiers(int[] ids);
}
