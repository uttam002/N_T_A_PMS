using System.Text;
using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class CommonServices : ICommonServices
{
    private readonly ICommonRepo _dalCommonServices;
    // private readonly IUserRepo _userService;
    // private readonly IHttpContextAccessor _httpContext;
    public CommonServices(ICommonRepo dalCommonServices)
    {
        _dalCommonServices = dalCommonServices;
        // _httpContext = httpContext;
        // _userService = userService;
    }
    ResponseResult result = new ResponseResult();

    public async Task<ResponseResult> GetCountryList()
    {
        try
        {
            result = await _dalCommonServices.GetCountryListAsync();
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetStateList(int countryId)
    {
        try
        {
            result = await _dalCommonServices.GetStateListAsync(countryId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetCityList(int stateId)
    {
        try
        {
            result = await _dalCommonServices.GetCityListAsync(stateId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetRoleList()
    {
        try
        {
            result = await _dalCommonServices.GetRoleListAsync();
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
     public async Task<string> Encrypt(string password)
    {
        byte[] encode = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(encode);
    }

    public Task<int> FindCurrentUserId()
    {
        throw new NotImplementedException();
    }

    // public async Task<int> FindCurrentUserId(){
    //     int userId= 0;
    //     try{
    //         string userEmail =_httpContext.HttpContext?.Session.GetString("Email");
    //         userId = await _userService.GetUserIdByEmailId(userEmail);
    //     }
    //     catch (Exception ex) {

    //      }
    //     return userId;
    // }
}
