using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class TableRepo : ITableRepo
{

    private readonly AppDbContext _appDbContext;


    public TableRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddTableAsync(TableDetails newTable)
    {
        try
        {
            var query = await _appDbContext.Tables.Where(c => c.TableName.ToLower() == newTable.TableName.ToLower()).FirstOrDefaultAsync();

            if (query != null)
            {
                result.Message = "Table is already Exist!!!";
                result.Status = ResponseStatus.Error;
                return result;
            }

            Table newRow = new Table();
            newRow.TableName = newTable.TableName;
            newRow.SectionId = newTable.SectionId;
            newRow.Capacity = newTable.Capacity;
            newRow.Status = newTable.Status;
            newRow.Createby = 9;
            newRow.Createat = DateTime.Now;
            newRow.Iscontinued = true;

            _appDbContext.Tables.Add(newRow);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Table Add successfully";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> UpdateTableAsync(TableDetails updateTable)
    {
        try
        {
            IQueryable<Table> query = _appDbContext.Tables.Where(c => c.TableId == updateTable.TableId && c.Iscontinued);
            Table existingTable = await query.FirstOrDefaultAsync();
            if (existingTable == null)
            {
                result.Message = "Table was Not Found";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                existingTable.TableName = updateTable.TableName;
                existingTable.SectionId = updateTable.SectionId;
                existingTable.Capacity = updateTable.Capacity;
                existingTable.Status = updateTable.Status;
                existingTable.Modifyby = 1;
                existingTable.Modifyat = DateTime.Now;

                _appDbContext.Tables.Update(existingTable);
                await _appDbContext.SaveChangesAsync();
                result.Message = "Update Tbale successfullly!!!";
                result.Status = ResponseStatus.Success;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<List<TableDetails>> GetTablesBySectionId(int id, PaginationDetails paginationDetails)
    {
        try
        {
            IQueryable<Table> query = _appDbContext.Tables
                             .Where(a => a.SectionId == id && a.Iscontinued == true);
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(o => o.TableName.Contains(paginationDetails.SearchQuery));
            }
            paginationDetails.totalRecords = await query.CountAsync();
            return await query.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                            .Take(paginationDetails.PageSize)
                                 .Select(c => new TableDetails
                                 {
                                     TableId = c.TableId,
                                     TableName = c.TableName,
                                     SectionId = c.SectionId,
                                     Capacity = c.Capacity,
                                     Status = c.Status
                                 }).ToListAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<ResponseResult> MassDeleteTablesAsync(List<int> tableIds)
    {
        try
        {
            foreach (int tableId in tableIds)
            {
                Table tableDetails = await _appDbContext.Tables.Where(t => t.TableId == tableId).FirstOrDefaultAsync();
                if (tableDetails != null)
                {
                    tableDetails.Iscontinued = false;
                    _appDbContext.Tables.Update(tableDetails);
                }
                else
                {
                    result.Message = "Some of selected Tables are not found!!!";
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
            }
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> DeleteTableAsync(int tableId)
    {
        try
        {
            Table existingTable = await _appDbContext.Tables.Where(t => t.TableId == tableId).FirstOrDefaultAsync();

            existingTable.Iscontinued = false;

            _appDbContext.Tables.Update(existingTable);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Table Delete Successfully !!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<Table> GetTableAsync(int tableId){
        try{
            return await _appDbContext.Tables.Where(t=>t.TableId == tableId).FirstOrDefaultAsync();
        }catch{
            return null;
        }
    }
}
