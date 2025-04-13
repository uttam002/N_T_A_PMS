using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class CommonRepo : ICommonRepo
{
    private readonly AppDbContext _context;
    public CommonRepo(AppDbContext context)
    {
        _context = context;
    }
    ResponseResult result = new ResponseResult();


    public async Task<ResponseResult> GetCountryListAsync()
    {
        try
        {
            var query = _context.ContryLists.Where(c => c.IsContinue == true);
            var countryList = query.ToList();
            if (countryList.Count > 0)
            {
                result.Data = countryList;
                result.Status = ResponseStatus.Success;
                result.Message = "Country List Found";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Country List Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetStateListAsync(int countryId)
    {
        try
        {
            var query = _context.StateLists.Where(s => s.ContryId == countryId).Select(c => new { c.StateId, c.StateName });
            var stateList = query.ToList();
            if (stateList.Count > 0)
            {
                result.Data = stateList;
                result.Status = ResponseStatus.Success;
                result.Message = "State List Found";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "State List Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> GetCityListAsync(int stateId)
    {
        try
        {
            var query = _context.CityLists.Where(c => c.StateId == stateId).Select(c => new { c.CityId, c.CityName });
            var cityList = query.ToList();
            if (cityList.Count > 0)
            {
                result.Data = cityList;
                result.Status = ResponseStatus.Success;
                result.Message = "City List Found";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "City List Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> GetRoleListAsync()
    {
        try
        {
            var query = _context.Roles;
            var roleList = query.ToList();
            if (roleList.Count > 0)
            {
                result.Data = roleList;
                result.Status = ResponseStatus.Success;
                result.Message = "Role List Found";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Role List Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> GetUserByEmail(string email){
        try{
            var query = _context.Userdetails.Include(u=>u.User).Where(u=>u.User.EmailId == email);
            var userData = await query.FirstOrDefaultAsync();
            if(userData != null){
                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = "User Found Successfully!!!";
            }
            else{
                result.Status = ResponseStatus.NotFound;
                result.Message = "User Not Found";
            }
        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status= ResponseStatus.Error;
        }
        return result;
    }
}
