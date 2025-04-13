using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class CustomerRepo : ICustomerRepo
{
    private readonly AppDbContext _appDbContext;

    public CustomerRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task<List<CustomerDetails>> GetCustomersAsync(PaginationDetails paginationDetails)
    {
        try
        {
            IQueryable<Order> query = _appDbContext.Orders.Include(o => o.Customer);

            // Apply search query filter
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(o => o.Customer.CustName.Contains(paginationDetails.SearchQuery));
            }

            // Apply date range filter
            if (paginationDetails.DateRange != timePeriod.All)
            {
                DateTime startDate = paginationDetails.DateRange switch
                {
                    timePeriod.LastSevenDays => DateTime.Now.AddDays(-7),
                    timePeriod.LastThirtyDays => DateTime.Now.AddDays(-30),
                    timePeriod.CurrentMonth => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    _ => DateTime.MinValue
                };

                query = query.Where(o => o.Createat >= startDate && o.Createat <= DateTime.Now);
            }

            // Apply custom date range filter
            DateTime fromDate = paginationDetails.FromDate.ToDateTime(new TimeOnly(0, 0));
            DateTime toDate = paginationDetails.ToDate.ToDateTime(new TimeOnly(23, 59, 59));

            if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
            {
                query = query.Where(o => o.Createat >= fromDate && o.Createat <= toDate);
            }

            // Group and project data
            IQueryable<CustomerDetails> groupedQuery = query.GroupBy(o => new
            {
                o.Customer.CustId,
                o.Customer.CustName,
                o.Customer.PhoneNumber,
                o.Customer.EmailId,
                o.Customer.TotalOrders
            })
            .Select(g => new CustomerDetails
            {
                CustomerId = g.Key.CustId,
                CustomerName = g.Key.CustName,
                CustomerPhone = g.Key.PhoneNumber,
                CustomerEmail = g.Key.EmailId,
                TotalOrders = g.Key.TotalOrders,
                LastOrder = DateOnly.FromDateTime(g.Max(o => o.Createat)) // Latest order date
            });

            // Apply sorting
            switch (paginationDetails.SortColumn.ToLower())
            {
                case "name":
                    if (paginationDetails.SortOrder == "asc")
                    {
                        groupedQuery = groupedQuery.OrderBy(c => c.CustomerName);
                    }
                    else
                    {
                        groupedQuery = groupedQuery.OrderByDescending(c => c.CustomerName);
                    }
                    break;

                case "totalorder":
                    if (paginationDetails.SortOrder == "asc")
                    {
                        groupedQuery = groupedQuery.OrderBy(c => c.TotalOrders);
                    }
                    else
                    {
                        groupedQuery = groupedQuery.OrderByDescending(c => c.TotalOrders);
                    }
                    break;

                case "date":
                    if (paginationDetails.SortOrder == "asc")
                    {
                        groupedQuery = groupedQuery.OrderBy(c => c.LastOrder);
                    }
                    else
                    {
                        groupedQuery = groupedQuery.OrderByDescending(c => c.LastOrder);
                    }
                    break;

                default:
                    groupedQuery = groupedQuery.OrderBy(c => c.CustomerName); // Default sorting
                    break;
            }

            // Set total records
            paginationDetails.totalRecords = await groupedQuery.CountAsync();

            // Fetch and return paginated data
            return await groupedQuery.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                     .Take(paginationDetails.PageSize)
                                     .ToListAsync();
        }
        catch
        {
            return null;
        }
    }
}
