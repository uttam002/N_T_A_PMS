using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class ItemModifierRepo : IItemModifierRepo
{
    private readonly AppDbContext _appDbContext;

    public ItemModifierRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddMultipleEntriesAsync(List<ItemModifierGroupRelation>? itemModifiers)
    {
        try
        {

            foreach (ItemModifierGroupRelation itemModifier in itemModifiers)
            {
                
                ItemModifierGroupsMapping newRow = new ItemModifierGroupsMapping();
                newRow.ItemId = itemModifier.ItemId;
                newRow.MgId = itemModifier.MgId;
                newRow.MinModifiers = itemModifier.MinModifiers;
                newRow.MaxModifiers = itemModifier.MaxModifiers;
                _appDbContext.ItemModifierGroupsMappings.Add(newRow);
                
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Item-Modifiers table Successfully Updated!!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<List<ItemModifierGroupRelation>> GetItemModifierDetails(int itemId)
    {
        List<ItemModifierGroupsMapping> query = await _appDbContext.ItemModifierGroupsMappings.Where(x => x.ItemId == itemId).ToListAsync();
        List<ItemModifierGroupRelation> itemModifierGroupRelations = new List<ItemModifierGroupRelation>();
        foreach (ItemModifierGroupsMapping item in query)
        {
            ItemModifierGroupRelation itemModifierGroupRelation = new ItemModifierGroupRelation();
            itemModifierGroupRelation.IMGid = item.ImId;
            itemModifierGroupRelation.ItemId = item.ItemId;
            itemModifierGroupRelation.MgId = item.MgId;
            itemModifierGroupRelation.MinModifiers = item.MinModifiers;
            itemModifierGroupRelation.MaxModifiers = item.MaxModifiers;
            itemModifierGroupRelations.Add(itemModifierGroupRelation);
        }
        return itemModifierGroupRelations;
    }

    public async Task<List<int>> GetRelationsByItemIdAsync (int itemId){
        List<int> relationIds = await _appDbContext.ItemModifierGroupsMappings.Where(x => x.ItemId == itemId).Select(x=>x.ImId).ToListAsync();
        return relationIds;
    }
    public async Task<ResponseResult> RemoveMultipleEntriesAsync(List<int> ids)
    {
        try
        {
            foreach (int id in ids)
            {
                ItemModifierGroupsMapping row = await _appDbContext.ItemModifierGroupsMappings.Where(x => x.ImId == id).FirstOrDefaultAsync();
                _appDbContext.ItemModifierGroupsMappings.Remove(row);
            }
            await _appDbContext.SaveChangesAsync();
            result.Status = ResponseStatus.Success;
            result.Message = "Delete successfully in Items Modifiers Relation";
        }
        catch (Exception ex)
        {
            result.Status = ResponseStatus.Error;
            result.Message = ex.Message;
        }
        return result;
    }

}
