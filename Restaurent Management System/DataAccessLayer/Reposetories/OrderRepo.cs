using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class OrderRepo : IOrderRepo
{
    private readonly AppDbContext _appDbContext;

    public OrderRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    ResponseResult result = new ResponseResult();

    public async Task<List<Order>> GetAllOrders()
    {
        try
        {
            // Query the orders for the given customer
            return await _appDbContext.Orders
                                      .Include(o => o.OrderDetails)
                                      .Include(o => o.InvoiceItemModifierMappings)
                                        .ThenInclude(i => i.Item)
                                      .Include(o => o.InvoiceItemModifierMappings)
                                        .ThenInclude(m => m.Modifier)
                                      .ToListAsync();

        }

        catch
        {
            return null;
        }
    }
    public async Task<List<Order>> GetOrdersByCategoryId(string status, int categoryId)
    {
        try
        {
            IQueryable<Order> query = _appDbContext.Orders
                .Include(o => o.OrderDetails)

                .Include(o => o.InvoiceItemModifierMappings)
                    .ThenInclude(i => i.Item)
                        .ThenInclude(i => i.Category)
                .Include(o => o.InvoiceItemModifierMappings)
                    .ThenInclude(m => m.Modifier)
                .Where(o => o.Status == "InProgress" || o.Status == "OnHold" || o.Status == "Pending");
            return await query
           .Select(o => new Order
           {
               OrderId = o.OrderId,
               CustomerId = o.CustomerId,
               Status = o.Status,
               Createat = o.Createat,
               Modifyat = o.Modifyat,
               OrderDetails = o.OrderDetails,
               InvoiceItemModifierMappings = status == "Ready" && categoryId != 0
                   ? o.InvoiceItemModifierMappings
                       .Where(m => m.Item.CategoryId == categoryId && m.PreparedItems == m.ItemQuantity)
                       .ToList()
                   : status == "Ready" && categoryId == 0
                   ? o.InvoiceItemModifierMappings
                       .Where(m => m.PreparedItems == m.ItemQuantity)
                       .ToList()
                   : status == "InProgress" && categoryId != 0
                   ? o.InvoiceItemModifierMappings
                       .Where(m => m.Item.CategoryId == categoryId && m.PreparedItems < m.ItemQuantity)
                       .ToList()
                   : o.InvoiceItemModifierMappings
                       .Where(m => m.PreparedItems < m.ItemQuantity)
                       .ToList()
           })
           .Where(o => o.InvoiceItemModifierMappings.Any()) // Only include orders with matching items
           .ToListAsync();



        }
        catch
        {
            return null;
        }
    }
    public async Task<ResponseResult> GetOrderDetailsAsync(int orderId)
    {
        try
        {
            // var order = await _appDbContext.Orders
            // .Include(o => o.Customer)
            // .Include(o => o.OrderDetails)
            //     .ThenInclude(od => od.Payment)
            // .Include(o => o.OrderDetails)
            //     .ThenInclude(od => od.Feedback)
            // .Include(o => o.OrderDetails)
            //     .ThenInclude(od => od.)
            //         .ThenInclude(oi => oi.ItemModifiers)
            // .Where(o => o.OrderId == orderId)
            // .Select(o => new OrderDetailsVM
            // {
            //     OrderId = o.OrderId,
            //     CustomerName = o.Customer.Name,
            //     Status = o.Status,
            //     DeliverOnTime = o.DeliverOnTime,
            //     NumberOfPersons = o.NuOfPersons,
            //     CreatedAt = o.CreateAt,
            //     ModifiedAt = o.ModifyAt,
            //     PaymentMethod = o.OrderDetails.Payment.PaymentMethod,
            //     TotalAmount = o.OrderDetails.Payment.TotalAmount,
            //     Feedback = o.OrderDetails.Feedback.Comments,
            //     OrderItems = o.OrderDetails.OrderItems.Select(oi => new OrderItemVM
            //     {
            //         ItemId = oi.ItemModiferId,
            //         ItemName = oi.Item.Name,  // Assuming there is a Name property in ItemModifiers
            //         Quantity = oi.Quantity,
            //         Price = oi.Prices,
            //         Modifiers = oi.ItemModifiers.Select(im => im.Name).ToList()  // Assuming there is a Name property in ItemModifiers
            //     }).ToList()
            // })
            // .FirstOrDefaultAsync();

            // return order;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<Dictionary<int, (int TotalOrders, DateOnly LastOrderDate)>> GetCustomerOrderDataAsync(List<int> customerIds)
    {
        // Query the database to fetch total orders and last order dates for the given customer IDs
        return await _appDbContext.Orders
            .Where(o => customerIds.Contains(o.CustomerId))
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalOrders = g.Count(),
                LastOrderDate = g.Max(o => o.Createat)
            })
            .ToDictionaryAsync(
                x => x.CustomerId,
                x => (
                    x.TotalOrders,
                    DateOnly.FromDateTime(x.LastOrderDate)
                )
            );
    }
    public async Task<List<OrderDetailsVM>> GetOrderListAsync(PaginationDetails paginationDetails)
    {
        try
        {
            // Include related entities
            IQueryable<OrderDetail> query = _appDbContext.OrderDetails
                                                        .Include(o => o.Order)
                                                            .ThenInclude(o => o.Customer)
                                                        .Include(o => o.Feedback)
                                                        .Include(o => o.Payment);

            // Apply search query filter
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(o => o.Payment.PaymentMethod.Contains(paginationDetails.SearchQuery) ||
                                         o.Order.Customer.CustName.ToLower().Contains(paginationDetails.SearchQuery) ||
                                         o.OrderId.ToString().Contains(paginationDetails.SearchQuery));
            }

            // Apply order status filter
            if (paginationDetails.OrderStatus != orderStatus.All)
            {
                query = query.Where(o => o.Order.Status == paginationDetails.OrderStatus.ToString());
            }

            // Apply date range filter
            if (paginationDetails.DateRange != timePeriod.All)
            {
                DateTime startDate;
                DateTime endDate = DateTime.Now;

                switch (paginationDetails.DateRange)
                {
                    case timePeriod.LastSevenDays:
                        startDate = endDate.AddDays(-7);
                        break;
                    case timePeriod.LastThirtyDays:
                        startDate = endDate.AddDays(-30);
                        break;
                    case timePeriod.CurrentMonth:
                        startDate = new DateTime(endDate.Year, endDate.Month, 1);
                        break;
                    default:
                        startDate = DateTime.MinValue;
                        break;
                }

                query = query.Where(o => o.Createdat >= startDate && o.Createdat <= endDate);
            }

            // Apply custom date range filter
            DateTime fromDate = paginationDetails.FromDate.ToDateTime(new TimeOnly(0, 0));
            DateTime toDate = paginationDetails.ToDate.ToDateTime(new TimeOnly(23, 59, 59));

            if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
            {
                query = query.Where(o => o.Createdat >= fromDate && o.Createdat <= toDate);
            }
            // Apply sorting
            switch (paginationDetails.SortColumn.ToLower())
            {
                case "id":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.OrderId) : query.OrderByDescending(o => o.OrderId);
                    break;
                case "date":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Createdat) : query.OrderByDescending(o => o.Createdat);
                    break;
                case "customer":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Order.Customer.CustName) : query.OrderByDescending(o => o.Order.Customer.CustName);
                    break;
                case "status":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Order.Status) : query.OrderByDescending(o => o.Order.Status);
                    break;
                case "paymetmethod":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Payment.PaymentMethod) : query.OrderByDescending(o => o.Payment.PaymentMethod);
                    break;
                case "totalamount":
                    query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(o => o.Payment.ActualPrice) : query.OrderByDescending(o => o.Payment.ActualPrice);
                    break;
                default:
                    query = query.OrderBy(o => o.OrderId); // Default sorting
                    break;
            }

            // Count filtered results
            paginationDetails.totalRecords = await query.CountAsync();

            if (paginationDetails.PageSize == 0) paginationDetails.PageSize = paginationDetails.totalRecords;
            // Apply pagination and project to OrderDetailsVM
            return await query
                            .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                            .Take(paginationDetails.PageSize)
                            .Select(o => new OrderDetailsVM
                            {
                                OrderId = o.Order.OrderId,
                                OrderDate = DateOnly.FromDateTime(o.Createdat),
                                CustomerName = o.Order.Customer.CustName,
                                OrderStatus = o.Order.Status,
                                PaymentType = o.Payment.PaymentMethod,
                                Rating = (short)((o.Feedback.FoodRating + o.Feedback.AmbianceRating + o.Feedback.ServiceRating) / 3),
                                TotalAmount = o.Payment.ActualPrice
                            })
                            .ToListAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<OrderDetail> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        try
        {
            return await _appDbContext.OrderDetails
                                        .Include(o => o.Order)
                                            .ThenInclude(c => c.Customer)
                                        .Include(p => p.Payment)
                                        .Where(o => o.OrderId == orderId)
                                        .FirstAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Order>> GetOrdersDataByCutomerIdAsync(int customerId)
    {
        try
        {
            // Query the orders for the given customer
            return await _appDbContext.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.PaymentDetails)
                                      .Include(o => o.InvoiceItemModifierMappings)
                                      .Where(o => o.CustomerId == customerId)
                                      .ToListAsync();

        }
        catch
        {
            return null;
        }
    }



}
