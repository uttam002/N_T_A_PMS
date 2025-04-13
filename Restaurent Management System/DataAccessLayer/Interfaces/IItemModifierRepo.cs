using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IItemModifierRepo
{
    Task<ResponseResult> AddMultipleEntriesAsync(List<ItemModifierGroupRelation>? itemModifiers);
    Task<List<ItemModifierGroupRelation>> GetItemModifierDetails(int itemId);
    Task<List<int>> GetRelationsByItemIdAsync(int itemId);
    Task<ResponseResult> RemoveMultipleEntriesAsync(List<int> ids);
}
