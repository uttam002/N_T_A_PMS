using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
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



    public async Task<ResponseResult> GetDefaultAreaDeatils(PaginationDetails paginationDetails)
    {
        try
        {
            List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
            List<Table> tables = await _tableRepo.GetTablesBySectionId(sections[0].SectionId, paginationDetails);
            List<SectionDetails> sectionList = await ConvertSectionToSectionDetailsViewModel(sections);
            List<TableDetails> tableList = await ConvertTablesToTableDetailsViewModel(tables);
            AreaDetails defaultArea = new AreaDetails
            {
                sections = sectionList,
                tables = tableList,
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




    #region  CRUD for Section
    public async Task<ResponseResult> AddSection(SectionDetails section)
    {
        try
        {
            Section newSection = new Section();
            newSection.SectionName = section.SectionName;
            newSection.Description = section.Description;
            newSection.Createdat = DateTime.Now;
            newSection.Createdby = section.editorId;
            newSection.Isdeleted = false;
            result = await _sectionRepo.AddSectionAsync(newSection);
            if (result.Status == ResponseStatus.Success)
            {
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                if (sections != null)
                {
                List<SectionDetails> sectionList = await ConvertSectionToSectionDetailsViewModel(sections);
                    result.Data = sectionList;
                    result.Message = "Section Successfully added!!!";
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = "Error During Fetach Section Data";
                    result.Status = ResponseStatus.NotFound;
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteSection(int sectionId, int editorId)
    {
        try
        {
            Section existingSection = await _sectionRepo.GetSectionAsync(sectionId);
            if (existingSection != null)
            {
                existingSection.Modifiedby = editorId;
                existingSection.Modifiedat = DateTime.Now;
                existingSection.Isdeleted = true;
            }
            result = await _sectionRepo.UpdateSectionAsync(existingSection);
            if (result.Status == ResponseStatus.Success)
            {
                result = await DeleteTablesBySectionId(sectionId, editorId);
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = "Section Deleted Successfully";
                }
                else
                {
                    result.Message = "Tables are not Deleted based on sectionId";
                    result.Status = ResponseStatus.Error;
                }
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                List<SectionDetails> sectionList = await ConvertSectionToSectionDetailsViewModel(sections);
                result.Data = sectionList;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    private async Task<ResponseResult> DeleteTablesBySectionId(int sectionId, int editorId)
    {
        List<Table> tables = await _tableRepo.GetTableListBySectionIdAsync(sectionId);
        foreach (Table table in tables)
        {
            table.Modifyat = DateTime.Now;
            table.Modifyby = editorId;
            table.Iscontinued = false;
        }
        result = await _tableRepo.MassUpdateTablesAsync(tables);
        return result;
    }

    public async Task<ResponseResult> EditSection(SectionDetails section)
    {
        try
        {
            Section existingSection = await _sectionRepo.GetSectionAsync(section.SectionId);
            if (existingSection != null)
            {
                existingSection.SectionName = section.SectionName;
                existingSection.Description = section.Description;
                existingSection.Modifiedby = section.editorId;
                existingSection.Modifiedat = DateTime.Now;
            }
            result = await _sectionRepo.UpdateSectionAsync(existingSection);
            if (result.Status == ResponseStatus.Success)
            {
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                if (sections != null)
                {
                    List<SectionDetails> sectionList = await ConvertSectionToSectionDetailsViewModel(sections);
                    result.Data = sectionList;
                    result.Message = "Section Successfully Updated!!!";
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = "Error During Fetach Section Data";
                    result.Status = ResponseStatus.NotFound;
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    #endregion


    #region  CRUD for Items

    public async Task<ResponseResult> GetTables(int sectionId, PaginationDetails paginationDetails)
    {
        try
        {
            List<Table> tableList = await _tableRepo.GetTablesBySectionId(sectionId, paginationDetails);
            List<TableDetails> Tables = await ConvertTablesToTableDetailsViewModel(tableList);
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


    #endregion

    public async Task<ResponseResult> AddTable(TableDetails newTable)
    {
        try
        {
            Table table = new Table();
            table.TableName = newTable.TableName;
            table.SectionId = newTable.SectionId;
            table.Capacity = newTable.Capacity;
            table.Status = newTable.Status;
            table.Createby = newTable.editorId;
            table.Createat = DateTime.Now;
            result = await _tableRepo.AddTableAsync(table);

            if (result.Status == ResponseStatus.Success)
            {
                PaginationDetails paginationDetails = new PaginationDetails();
                paginationDetails.PageSize = 2;
                List<Table> tables = await _tableRepo.GetTablesBySectionId(newTable.SectionId, paginationDetails);
                if (tables == null)
                {
                    result.Message = "Error During Updated TableList";
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<TableDetails> tableList = await ConvertTablesToTableDetailsViewModel(tables);
                    result.Message = "Table Added Successfully!!!";
                    result.Status = ResponseStatus.Success;
                    result.Data = (tableList, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteTable(int tableId, int editorId)
    {
        try
        {
            Table table = await _tableRepo.GetTableAsync(tableId);
            table.Modifyby = editorId;
            table.Modifyat = DateTime.Now;
            table.Iscontinued = false;
            result = await _tableRepo.UpdateTableAsync(table);

            if (result.Status == ResponseStatus.Success)
            {
                PaginationDetails paginationDetails = new PaginationDetails();
                paginationDetails.PageSize = 2;
                List<Table> tables = await _tableRepo.GetTablesBySectionId(table.SectionId, paginationDetails);
                if (tables == null)
                {
                    result.Message = "Error During Updated TableList";
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<TableDetails> tableList = await ConvertTablesToTableDetailsViewModel(tables);
                    result.Message = "Table Deleted Successfully!!!";
                    result.Status = ResponseStatus.Success;
                    result.Data = (tableList, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> UpdateTable(TableDetails updateTable)
    {
        try
        {
            Table table = await _tableRepo.GetTableAsync(updateTable.TableId);
            table.TableName = updateTable.TableName;
            table.SectionId = updateTable.SectionId;
            table.Capacity = updateTable.Capacity;
            table.Status = updateTable.Status;
            table.Modifyby = updateTable.editorId;
            table.Modifyat = DateTime.Now;
            result = await _tableRepo.UpdateTableAsync(table);

            if (result.Status == ResponseStatus.Success)
            {
                PaginationDetails paginationDetails = new PaginationDetails();
                paginationDetails.PageSize = 2;
                List<Table> tables = await _tableRepo.GetTablesBySectionId(updateTable.SectionId, paginationDetails);
                if (tables == null)
                {
                    result.Message = "Error During Updated TableList";
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<TableDetails> tableList = await ConvertTablesToTableDetailsViewModel(tables);
                    result.Message = "Table Updated Successfully!!!";
                    result.Status = ResponseStatus.Success;
                    result.Data = (tableList, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> MassDeleteTableAsync(int[] tableIds, int editorId)
    {
        try
        {
            List<Table> tables = await _tableRepo.GetTableListFromTableIdsAsync(tableIds);
            foreach (Table table in tables)
            {
                table.Modifyat = DateTime.Now;
                table.Modifyby = editorId;
                table.Iscontinued = false;
            }
            result = await _tableRepo.MassUpdateTablesAsync(tables);
            if (result.Status == ResponseStatus.Success)
            {
                
                PaginationDetails paginationDetails = new PaginationDetails();
                paginationDetails.PageSize = 2;
                List<Table> updatedtables = await _tableRepo.GetTablesBySectionId(tables[0].SectionId, paginationDetails);
                if (tables == null)
                {
                    result.Message = "Error During Updated TableList";
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<TableDetails> tableList = await ConvertTablesToTableDetailsViewModel(tables);
                    result.Message = "Mass Delete Successfully Done!!!";
                    result.Status = ResponseStatus.Success;
                    result.Data = (tableList, paginationDetails);
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    //Convert Entity to ViewModel 
    private async Task<List<TableDetails>> ConvertTablesToTableDetailsViewModel(List<Table> tables)
    {
        List<TableDetails> tableDetails = new List<TableDetails>();
        foreach (Table table in tables)
        {
            TableDetails temp = new TableDetails();
            temp.TableId = table.TableId;
            temp.TableName = table.TableName;
            temp.SectionId = table.SectionId;
            temp.Capacity = table.Capacity;
            temp.TableName = table.TableName;
            temp.Status = table.Status;
            tableDetails.Add(temp);
        }
        return tableDetails;
    }

    private async Task<List<SectionDetails>> ConvertSectionToSectionDetailsViewModel(List<Section> sections)
    {
        List<SectionDetails> sectiondetails = new List<SectionDetails>();
        foreach (Section section in sections)
        {
            SectionDetails temp = new SectionDetails();
            temp.SectionId = section.SectionId;
            temp.SectionName = section.SectionName;
            temp.Description = section.Description;
            sectiondetails.Add(temp);
        }
        return sectiondetails;
    }


}
