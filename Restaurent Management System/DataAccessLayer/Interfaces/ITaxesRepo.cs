using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface ITaxesRepo
{
    Task<ResponseResult> AddNewTaxAync(TaxDetails taxDetails);

    Task<ResponseResult> DeleteTaxAync(int taxId,int editorId);

    Task<ResponseResult> GetAllTaxesAsync(PaginationDetails paginationDetails);
    Task<ResponseResult> UpdateTaxAsync(TaxDetails taxDetails);
    Task<Taxis> GetTaxAsync(int taxId);
}
