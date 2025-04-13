using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class SectionAndTablesService : ISectionAndTablesService
{
    private readonly ISectionRepo _sectionRepo;
    private readonly ITableRepo _tableRepo;
    private readonly ICommonServices _commonServices;

    public SectionAndTablesService(ISectionRepo sectionRepo, ITableRepo tableRepo, ICommonServices commonServices)
    {
        _sectionRepo = sectionRepo;
        _tableRepo = tableRepo;
        _commonServices = commonServices;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddSection(string sectionName, string description)
    {
       return await _sectionRepo.AddSectionAsync(sectionName, description);
    }

    public Task<ResponseResult> DeleteSection(int sectionId,int editorId)
    {
        return _sectionRepo.DeleteSectionBySectionId(sectionId,editorId);
    }

    public async Task<ResponseResult> EditSection(int sectionId, string sectionName, string description,int editorId)
    {
        return await _sectionRepo.EditSectionAsync(sectionId, sectionName, description,editorId);
    }


    public async Task<ResponseResult> GetTables(int sectionId ,PaginationDetails paginationDetails)
    {
        try
        {
            List<TableDetails> Tables = await _tableRepo.GetTablesBySectionId(sectionId,paginationDetails);
            if (Tables.Count > 0)
            {
                result.Message = "Sucessfully got Tables";
                result.Status = ResponseStatus.Success;
                result.Data = Tables;
            }
            else
            {
                result.Message = "This Section don't have any Tables";
                result.Status = ResponseStatus.NotFound;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> GetDefaultAreaDeatils(PaginationDetails paginationDetails)
    {
        try
        {
            List<SectionDetails> sectionList = await _sectionRepo.GetAllSectonsAsync();
            List<TableDetails> tablesForFirstSection = await _tableRepo.GetTablesBySectionId(sectionList[0].SectionId,paginationDetails);

            AreaDetails defaultArea = new AreaDetails
            {
                sections = sectionList,
                tables = tablesForFirstSection,
            };
            if (defaultArea != null)
            {
                result.Message = "Sucessfully got Area Details";
                result.Status = ResponseStatus.Success;
                result.Data = defaultArea;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> AddTable(TableDetails newTable)
    {
        return await _tableRepo.AddTableAsync(newTable);
    }

    public async Task<ResponseResult> DeleteTable(int tableId){
        return await _tableRepo.DeleteTableAsync(tableId);
    }

    public async Task<ResponseResult> UpdateTable(TableDetails updateTable){
        try{
            // updateTable.editorId = await _commonServices.FindCurrentUserId();
            result = await _tableRepo.UpdateTableAsync(updateTable);
        }
        catch(Exception ex){
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
}
