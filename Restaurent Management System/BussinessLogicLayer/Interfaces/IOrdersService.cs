using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSServices.Interfaces;

public interface IOrdersService
{
    Task<(byte[], string)> CreatePdfAsync(int orderId,string partialView);

    Task<ResponseResult> ExportOrderList(string orderSearch, string status, string dateRange);
    Task<ResponseResult> GetOrderDetailsAsync(int orderId);
    Task<(string, string)> GetTableBasedOrdersDetails(int[] tableIds);
    Task<ResponseResult> GetOrderList(PaginationDetails paginationDetails);
}
