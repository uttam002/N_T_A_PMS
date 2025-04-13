using Azure;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class ItemModifierService : IItemModifierService
{
    private readonly IItemModifierRepo _itemModifierRepo;

    public ItemModifierService(IItemModifierRepo itemModifierRepo){
        _itemModifierRepo = itemModifierRepo;
    }

ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddMultipleEntriesOfItemModifier(List<ItemModifierGroupRelation>? itemModifiers)
    {
        try{
            if(itemModifiers == null){
                result.Message = "Item-Modifiers List Found As Empty Box!!!";
                result.Status = ResponseStatus.NotFound;
            }
            result = await _itemModifierRepo.AddMultipleEntriesAsync(itemModifiers);
        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> UpdateItemModifiers(List<ItemModifierGroupRelation> itemModifierGroupRelations, int itemId ){
        try{
            //Take a list of existing relations
            List<int> existingRelationIds = await _itemModifierRepo.GetRelationsByItemIdAsync(itemId);
            List<int> newRelationIds = new List<int>();
            foreach(ItemModifierGroupRelation relation in itemModifierGroupRelations){
                if(relation.IMGid == 0) continue;
                newRelationIds.Add(relation.IMGid);
            }
            List<int> removeRelations = existingRelationIds.Except(newRelationIds).ToList();
            result = await _itemModifierRepo.RemoveMultipleEntriesAsync(removeRelations);
            if(result.Status == ResponseStatus.Success){
                result = await _itemModifierRepo.AddMultipleEntriesAsync(itemModifierGroupRelations);
            }
            else{
                result.Status = ResponseStatus.Error;
                result.Message = "Error face during delete extra relations";
            }


        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status= ResponseStatus.Error;
        }
        return result;
    }
    public async Task<List<ItemModifierGroupRelation>> GetItemModifierDetails(int itemId)
    {
        return await _itemModifierRepo.GetItemModifierDetails(itemId);
    }
}

