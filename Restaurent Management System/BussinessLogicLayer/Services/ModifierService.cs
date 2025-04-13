using Microsoft.EntityFrameworkCore.Metadata;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class ModifierService : IModifierService
{
    private readonly IModifierRepo _modifierRepo;

    public ModifierService(IModifierRepo modifierRepo){
        _modifierRepo = modifierRepo;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddNewModifier(AddModifier newModifer)
    {
        return await _modifierRepo.AddModifierAsync(newModifer);
    }

    public async Task<ResponseResult> DeleteModifierByModifierId(int modifierId)
    {
        return await _modifierRepo.DeleteModifierByModifierId(modifierId);
    }

    public async Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId)
    {
        return await _modifierRepo.DeleteModifierGroupByModifierGroupId(modifierGroupId);
    }

    public async Task<List<ModifierGropDetails>> GetAllModifierGroups(){
        return await _modifierRepo.GetAllModifierGroups();
    }

    public async Task<List<ModifierDetails>> GetModifiersByModifierGroupId(int id, PaginationDetails paginationDetails){
        return await _modifierRepo.GetModifiersByModifierGroupId(id,paginationDetails);
    }

    public async Task<ResponseResult> GetModifierByModifierId(int modifierId)
    {
        return await _modifierRepo.GetModifierByModifierIdAsync(modifierId);
    }

    public async Task<ResponseResult> EditModifier(AddModifier editModifier)
    {
        return await _modifierRepo.EditModifierAsync(editModifier);
    }

    public Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails)
    {
        return _modifierRepo.GetAllModifiers(paginationDetails);
    }

  


    
    public async Task<ResponseResult> DeleteMultipleModifiers(int[] ids){
       return await _modifierRepo.DeleteMultipleModifiersAsync(ids);
    }
    public async Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId)
    {
        return await _modifierRepo.GetModifiersByModifierGroupIdAsync(groupId);
    }

    public Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM newGroup)
    {
        throw new NotImplementedException();
    }

}
 