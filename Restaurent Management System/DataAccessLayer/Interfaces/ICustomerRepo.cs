using Azure;
using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface ICustomerRepo
{
    Task<List<CustomerDetails>> GetCustomersAsync(PaginationDetails paginationDetails);
}
