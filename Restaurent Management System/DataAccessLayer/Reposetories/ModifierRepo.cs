using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class ModifierRepo : IModifierRepo
{

    private readonly AppDbContext _appDbContext;

    public ModifierRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();


    #region Modifier Group

    public async Task<ResponseResult> AddModifierGroupAsync(ModifiersGroup newGroup)
    {
        try
        {

            _appDbContext.ModifiersGroups.Add(newGroup);
            await _appDbContext.SaveChangesAsync();

            result.Message = "Modifier-Group Successfully saved!!!";
            result.Status = ResponseStatus.Success;

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<List<ModifierGropDetails>> GetAllModifierGroups()
    {
        return await _appDbContext.ModifiersGroups
                                .OrderBy(g => g.MgId)
                                .Where(g => g.Isactive == true)
                                .Select(g => new ModifierGropDetails { id = g.MgId, modifierGroupName = g.MgName, description = g.Description })
                                .ToListAsync();
    }
    public async Task<ResponseResult> UpdateModifierGroupAsync(ModifiersGroup modifiersGroup){
        try
        {
            _appDbContext.ModifiersGroups.Update(modifiersGroup);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Modifier Group is successfully updated";
            result.Status = ResponseStatus.Success;

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ModifiersGroup> GetModifierGroupById(int id)
    {
        ModifiersGroup? modifierGroup = await _appDbContext.ModifiersGroups.FirstOrDefaultAsync(i => i.MgId == id && i.Isactive == true);
        return modifierGroup;
    }
    #endregion
    #region Modifier

    #endregion
    #region Modifier Group Relation
    
    public async Task<ResponseResult> AddNewModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> newGroupMapping)
    {
        try
        {
            foreach (ModifierModifierGroupRelation newRow in newGroupMapping)
            {
                _appDbContext.ModifierModifierGroupRelations.Add(newRow);
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Modifier-ModifierGroup relations Successfully saved!!!";
            result.Status = ResponseStatus.Success;

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> RemoveModifierAndGroupRelationsAsync(List<ModifierModifierGroupRelation> existingRelations)
    {
        try
        {
            foreach (ModifierModifierGroupRelation existingRow in existingRelations)
            {
                _appDbContext.ModifierModifierGroupRelations.Remove(existingRow);
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Modifier-ModifierGroup relations Successfully removed!!!";
            result.Status = ResponseStatus.Success;

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<List<ModifierModifierGroupRelation>> GetModifiersRelationsByGroupid(int groupId)
    {
        return await _appDbContext.ModifierModifierGroupRelations.Where(m => m.GroupId == groupId).ToListAsync();
    }

    #endregion
    
    public async Task<ResponseResult> DeleteModifierGroupByModifierGroupId(int modifierGroupId)
    {
        var modifiersGroup = await _appDbContext.ModifiersGroups.FirstOrDefaultAsync(i => i.MgId == modifierGroupId);
        if (modifiersGroup == null)
        {
            result.Message = "Modifier Group is not Found";
            result.Status = ResponseStatus.NotFound;
            return result;
        }
        modifiersGroup.Isactive = false;
        await _appDbContext.SaveChangesAsync();
        // DeleteAllItemsByModifierGroupId(id);
        result.Message = "Category is successfully deleted";
        result.Status = ResponseStatus.Success;
        return result;
    }


    public async Task<List<ModifierDetails>> GetModifiersByModifierGroupId(int id, PaginationDetails paginationDetails)
    {
        var query = _appDbContext.ModifierModifierGroupRelations
                                    .Include(m => m.Modifier)
                                    .Include(m => m.Group)
                                    .Where(g => g.GroupId == id && g.Modifier.Isactive == true);
        if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
        {
            query = query.Where(a => a.Modifier.MName.Contains(paginationDetails.SearchQuery));
        }
        paginationDetails.totalRecords = await query.CountAsync();
        return await query
                     .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                     .Take(paginationDetails.PageSize)
                     .Select(m => new ModifierDetails
                     {
                         id = m.Modifier.MId,
                         modifierName = m.Modifier.MName,
                         unitPrice = m.Modifier.UnitPrice,
                         unitType = m.Modifier.UnitType,
                         quantity = m.Modifier.Quantity,
                         groupId = m.Group.MgId,
                         description = m.Modifier.Description
                     })
                     .ToListAsync();
    }
    public async Task<ResponseResult> DeleteModifierByModifierId(int modifierId)
    {
        var modifier = await _appDbContext.Modifiers.FirstOrDefaultAsync(i => i.MId == modifierId);
        if (modifier == null)
        {
            result.Message = "modifier is not Found";
            result.Status = ResponseStatus.NotFound;
            return result;
        }
        modifier.Isactive = false;
        _appDbContext.Modifiers.Update(modifier);
        await _appDbContext.SaveChangesAsync();
        result.Message = "Item is successfully deleted";
        result.Status = ResponseStatus.Success;
        return result;
    }
    public async Task<ResponseResult> AddModifierAsync(AddModifier newModifer)
    {
        Modifier query = await _appDbContext.Modifiers.Where(i => i.MName.ToLower() == newModifer.modifierName.ToLower()).FirstOrDefaultAsync();

        if (query == null)
        {
            Modifier temp = new Modifier();
            temp.MId = await _appDbContext.Modifiers.CountAsync() + 1;
            temp.MName = newModifer.modifierName;
            // temp.MgId = newModifer.ModifierGroupId;
            temp.UnitPrice = newModifer.UnitPrice;
            temp.Quantity = newModifer.Quantity;
            temp.UnitType = newModifer.UnitType;
            temp.Description = newModifer.Description;
            temp.Createat = DateTime.Now;
            temp.Createby = 9;
            temp.Isactive = true;
            _appDbContext.Modifiers.Add(temp);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Modifer is successfully Added";
            result.Status = ResponseStatus.Success;
        }
        else
        {
            result.Message = "Modifer is successfully Updated";
            result.Status = ResponseStatus.NotFound;
        }

        return result;
    }
    public async Task<ResponseResult> EditModifierAsync(AddModifier editModifer)
    {
        var query = await _appDbContext.Modifiers.Where(i => i.MId == editModifer.modifierId).FirstOrDefaultAsync();

        if (query == null)
        {
            result.Message = "NOT Found this Modifer";
            result.Status = ResponseStatus.NotFound;
        }
        else
        {
            query.MName = editModifer.modifierName;
            // query.MgId = editModifer.ModifierGroupId;
            query.UnitPrice = editModifer.UnitPrice;
            query.Quantity = editModifer.Quantity;
            query.UnitType = editModifer.UnitType;
            query.Description = editModifer.Description;
            query.Modifyat = DateTime.Now;
            query.Modifyby = 9;
            _appDbContext.Modifiers.Add(query);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Modifer is successfully Updated";
            result.Status = ResponseStatus.Success;
        }

        return result;
    }
    public async Task<ResponseResult> GetModifierByModifierIdAsync(int modifierId)
    {
        var modifier = await _appDbContext.Modifiers.FirstOrDefaultAsync(i => i.MId == modifierId);
        if (modifier == null)
        {
            result.Message = "modifier is not Found";
            result.Status = ResponseStatus.NotFound;
            return result;
        }
        result.Message = "Item is successfully fetched";
        result.Status = ResponseStatus.Success;
        result.Data = modifier;
        return result;
    }
    public async Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails)
    {
        try
        {
            IQueryable<Modifier> query = _appDbContext.Modifiers;

            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                paginationDetails.SearchQuery = paginationDetails.SearchQuery.Trim().ToLower();
                query = query.Where(u => u.MName.ToLower().Contains(paginationDetails.SearchQuery));
            }

            paginationDetails.totalRecords = await query.CountAsync(); // Count filtered results

            return await query.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                .Take(paginationDetails.PageSize)
                                .Select(m => new ModifierDetails
                                {
                                    id = m.MId,
                                    modifierName = m.MName,
                                    unitPrice = m.UnitPrice,
                                    unitType = m.UnitType,
                                    quantity = m.Quantity,
                                    description = m.Description
                                }).ToListAsync();

        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public async Task<string> GetModifierNameByIdAsync(int modifierId)
    {

        string? modifierName = await _appDbContext.Modifiers.Where(x => x.MId == modifierId).Select(i => i.MName).FirstOrDefaultAsync();
        return modifierName;
    }
    public async Task<ResponseResult> DeleteMultipleModifiersAsync(int[] ids)
    {
        try
        {
            foreach (int id in ids)
            {
                Modifier modifier = await GetModifierById(id);
                modifier.Isactive = false;
                modifier.Modifyat = DateTime.Now;
                _appDbContext.Modifiers.Update(modifier);
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Delete Multiple selected Modifiers";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        return result;

    }
    public async Task<Modifier> GetModifierById(int modifierId)
    {
        Modifier modifier = await _appDbContext.Modifiers.Where(x => x.MId == modifierId && x.Isactive == true).FirstOrDefaultAsync();
        return modifier;
    }
    public async Task<List<ModifierDetails>> GetModifiersByModifierGroupIdAsync(int groupId)
    {
        return await _appDbContext.ModifierModifierGroupRelations
                                    .Include(m => m.Modifier)
                                    .Where(m => m.GroupId == groupId)
                                    .Select(m => new ModifierDetails
                                    {
                                        id = m.ModifierId,
                                        modifierName = m.Modifier.MName,
                                        unitPrice = m.Modifier.UnitPrice
                                    })
                                    .ToListAsync();
    }

    public Task<int> GetModifierGroupIdByName(string groupName)
    {
        throw new NotImplementedException();
    }

}

