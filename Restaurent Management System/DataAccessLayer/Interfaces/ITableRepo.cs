using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface ITableRepo
{
    Task<ResponseResult> AddTableAsync(TableDetails newTable);
    Task<List<TableDetails>> GetTablesBySectionId(int id,PaginationDetails paginationDetails);
    Task<ResponseResult> DeleteTableAsync(int tableId);
    Task<ResponseResult> MassDeleteTablesAsync(List<int> tableIds);
    Task<ResponseResult> UpdateTableAsync(TableDetails updateTable);
    Task<Table> GetTableAsync(int tableId);
}
