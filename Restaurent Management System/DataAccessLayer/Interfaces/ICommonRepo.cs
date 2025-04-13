using PMSCore.Beans;

namespace PMSData.Interfaces;

public interface ICommonRepo
{
    Task<ResponseResult> GetCountryListAsync();
    Task<ResponseResult> GetStateListAsync(int countryId);
    Task<ResponseResult> GetCityListAsync(int stateId);
    Task<ResponseResult> GetRoleListAsync();
    Task<ResponseResult> GetUserByEmail(string email);
}
