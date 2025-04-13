using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class Taxesrepo : ITaxesRepo
{
    private readonly AppDbContext _appDbContext;

    public Taxesrepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> GetAllTaxesAsync(PaginationDetails paginationDetails)
    {
        try
        {
            IQueryable<Taxis> query = _appDbContext.Taxes.Where(t => t.Iscontinued == true);
             if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(o => o.TaxName.Contains(paginationDetails.SearchQuery));
            }
            paginationDetails.totalRecords = await query.CountAsync();
            List<TaxDetails> listOfTaxes = await query
                                                    .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                                    .Take(paginationDetails.PageSize)
                                                    .Select(t => new TaxDetails
                                                    {
                                                        TaxName = t.TaxName,
                                                        TaxType = t.TaxType,
                                                        TaxId = t.TaxId,
                                                        Isenabled = t.Isenabled ?? false,
                                                        TaxValue = t.TaxValue,
                                                        Isdefault = t.Isdefault
                                                    })
                                                    .ToListAsync();
            if (listOfTaxes != null)
            {
                result.Data = listOfTaxes;
                result.Message = "List of Taxes are fetched!!!";
                result.Status = ResponseStatus.Success;
            }
            else
            {
                result.Message = "List of Taxes are Not Found!!!";
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

    public async Task<ResponseResult> AddNewTaxAync(TaxDetails taxDetails)
    {
         try
        {
            Taxis query = await _appDbContext.Taxes.Where(c => c.TaxName.ToLower() == taxDetails.TaxName.ToLower()).FirstOrDefaultAsync();

            if (query != null)
            {
                result.Message = "Table is already Exist!!!";
                result.Status = ResponseStatus.Error;
                return result;
            }

            Taxis newRow = new Taxis();
            newRow.TaxName = taxDetails.TaxName;
            newRow.TaxValue = taxDetails.TaxValue;
            newRow.TaxType = taxDetails.TaxType;
            newRow.Isdefault = taxDetails.Isdefault;
            newRow.Isdefault = taxDetails.Isdefault;
            newRow.Createby = taxDetails.EditorId;
            newRow.Createat = DateTime.Now;
            newRow.Iscontinued = true;

            _appDbContext.Taxes.Add(newRow);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Tax Add successfully";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> DeleteTaxAync(int taxId,int editorId)
    {
        try
        {
            Taxis existingTax = await _appDbContext.Taxes.Where(t => t.TaxId == taxId).FirstOrDefaultAsync();

            existingTax.Iscontinued = false;
            existingTax.Modifyby = editorId;
            existingTax.Modifyat = DateTime.Now;

            _appDbContext.Taxes.Update(existingTax);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Tax Delete Successfully !!!";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> UpdateTaxAsync(TaxDetails taxDetails)
    {
        try
        {
            IQueryable<Taxis> query = _appDbContext.Taxes.Where(c => c.TaxId == taxDetails.TaxId && c.Iscontinued);
            Taxis existingTax = await query.FirstOrDefaultAsync();
            if (existingTax == null)
            {
                result.Message = "Tax was Not Found";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                existingTax.TaxValue = taxDetails.TaxValue;
                existingTax.TaxName = taxDetails.TaxName;
                existingTax.TaxType = taxDetails.TaxType;
                existingTax.Isdefault = taxDetails.Isdefault;
                existingTax.Isenabled = taxDetails.Isenabled;
                existingTax.Modifyby = taxDetails.EditorId;
                existingTax.Modifyat = DateTime.Now;

                _appDbContext.Taxes.Update(existingTax);
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

    public async Task<Taxis> GetTaxAsync(int taxId){
        try{
            return await _appDbContext.Taxes.Where(t=>t.TaxId == taxId).FirstOrDefaultAsync();
        }catch{
            return null;
        }
    }
}
