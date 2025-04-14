using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces;

public interface IOrderAppService
{
    Task<ResponseResult> GetKOTs(string status,int categoryId);
    Task<ResponseResult> UpdateKOT(List<KOTVM.KOTItemsVM> kotItems, int orderId);

}
