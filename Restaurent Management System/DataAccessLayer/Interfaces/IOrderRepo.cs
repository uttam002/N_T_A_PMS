using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IOrderRepo
{
    Task<Dictionary<int, (int TotalOrders, DateOnly LastOrderDate)>> GetCustomerOrderDataAsync(List<int> customerIds);

    Task<ResponseResult> GetOrderDetailsAsync(int orderId);
    Task<OrderDetail> GetOrderDetailsByOrderIdAsync(int orderId);
    Task<List<Order>> GetOrdersByCategoryId(string status,int categoryId);
    Task<List<OrderDetailsVM>> GetOrderListAsync(PaginationDetails paginationDetails);
    // Task<List<OrderItemsList>> GetOrderItemsListByOrderIdAsync(int orderId);
    Task<List<Order>> GetOrdersDataByCutomerIdAsync(int customerId);
}
