using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces;

public interface IModifierService
{
    Task<ResponseResult> AddNewModifier(AddModifier newModifer);
    Task<ResponseResult> DeleteModifierByModifierId(int modifierId);
    Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId);
    Task<List<ModifierGropDetails>> GetAllModifierGroups();
    Task<List<ModifierDetails>> GetModifiersByModifierGroupId(int id,PaginationDetails paginationDetails);
    Task<ResponseResult> GetModifierByModifierId(int modifierId);
    Task<ResponseResult> EditModifier(AddModifier editModifier);
    Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails);
    Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup);
    Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM newGroup);
    Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId);
    Task<ResponseResult> DeleteMultipleModifiers(int[] ids);
}
