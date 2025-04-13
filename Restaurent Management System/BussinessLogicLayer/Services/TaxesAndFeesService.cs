using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class TaxesAndFeesService : ITaxesAndFeesService
{
    private readonly ITaxesRepo _taxRepo;

    public TaxesAndFeesService(ITaxesRepo taxRepo){
        _taxRepo = taxRepo;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> AddNewTax(TaxDetails taxDetails)
    {
        try
        {
            result = await _taxRepo.AddNewTaxAync(taxDetails);
            if(result.Status == ResponseStatus.Success)
            {
                PaginationDetails paginationDetails = new PaginationDetails();
                result = await GetTaxes(paginationDetails);
                return result;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return await _taxRepo.AddNewTaxAync(taxDetails);
    }

    public async Task<ResponseResult> DeleteTax(int taxId,int editorId)
    {
        return await _taxRepo.DeleteTaxAync(taxId, editorId);
    }

    public async Task<ResponseResult> GetTaxes(PaginationDetails paginationDetails)
    {
        return await _taxRepo.GetAllTaxesAsync(paginationDetails);
    }

    public async Task<ResponseResult> UpdateTax(TaxDetails taxDetails)
    {
        return await _taxRepo.UpdateTaxAsync(taxDetails);
    }

}
