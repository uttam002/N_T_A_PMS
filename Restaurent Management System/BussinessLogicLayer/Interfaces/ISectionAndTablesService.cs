using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces;

public interface ISectionAndTablesService
{
    Task<ResponseResult> GetDefaultAreaDeatils(PaginationDetails paginationDetails);
    Task<ResponseResult> GetTables(int sectionId, PaginationDetails paginationDetails);
    Task<ResponseResult> AddSection(string sectionName, string description);

    Task<ResponseResult> EditSection(int sectionId, string sectionName, string description,int editorId);
    Task<ResponseResult> DeleteSection(int sectionId,int editorId);
    Task<ResponseResult> AddTable(TableDetails newTable);
    Task<ResponseResult> DeleteTable(int tableId);
    Task<ResponseResult> UpdateTable(TableDetails updateTable);
}
