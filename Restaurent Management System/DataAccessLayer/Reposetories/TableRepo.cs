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
    public async Task<ResponseResult> AddTableAsync(Table newTable)
    {
        try
        {
            _appDbContext.Tables.Add(newTable);
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
    public async Task<ResponseResult> UpdateTableAsync(Table updateTable)
    {
        try
        {
            _appDbContext.Tables.Update(updateTable);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Update Table successfullly!!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<List<Table>> GetTablesBySectionId(int id, PaginationDetails paginationDetails)
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
                                 .ToListAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<Table> GetTableAsync(int tableId)
    {
        try
        {
            return await _appDbContext.Tables.Where(t => t.TableId == tableId).FirstOrDefaultAsync();
        }
        catch
        {
            return null;
        }
    }
    public async Task<ResponseResult> MassUpdateTablesAsync(List<Table> tableList)
    {
        try
        {
            foreach (Table table in tableList)
            {
                _appDbContext.Tables.Update(table);
            }
            await _appDbContext.SaveChangesAsync();
            result.Message = "Mass Update Tables Successfully!!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<List<Table>> GetTableListBySectionIdAsync(int sectionId)
    {
        return await _appDbContext.Tables.Where(s => s.SectionId == sectionId).ToListAsync();
    }

    public async Task<List<Table>> GetTableListFromTableIdsAsync(int[] tableIds)
    {
        return await _appDbContext.Tables.Where(table => tableIds.Contains(table.TableId)).ToListAsync();
    }
}
