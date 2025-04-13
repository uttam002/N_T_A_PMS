using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IModifierRepo
{
    Task<ResponseResult> AddModifierAsync(AddModifier newModifer);
    Task<ResponseResult> AddModifierGroupAsync(ModifiersGroup newGroup);
    Task<ResponseResult> DeleteModifierByModifierId(int modifierId);
    Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId);
    Task<ResponseResult> EditModifierAsync(AddModifier editModifier);
    Task<List<ModifierModifierGroupRelation>> GetModifiersRelationsByGroupid(int groupId);
    Task<ResponseResult> RemoveModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> existingRelations);
    Task<ResponseResult> AddNewModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> newGroupMapping);
    Task<List<ModifierGropDetails>> GetAllModifierGroups();
    Task<ModifiersGroup> GetModifierGroupById(int id);
    Task<ResponseResult> UpdateModifierGroupAsync(ModifiersGroup updateGroup);
    Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails);
    Task<ResponseResult> GetModifierByModifierIdAsync(int modifierId);
    Task<string> GetModifierNameByIdAsync(int modifierId);
    Task<List<ModifierDetails>> GetModifiersByModifierGroupId(int id, PaginationDetails paginationDetails);
    Task<List<ModifierDetails>> GetModifiersByModifierGroupIdAsync(int groupId);
    Task<ResponseResult> DeleteMultipleModifiersAsync(int[] ids);
    Task<int> GetModifierGroupIdByName(string groupName);

}
