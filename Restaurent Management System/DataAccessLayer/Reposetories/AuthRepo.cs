using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using System.Text;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace PMSData.Reposetories;

public class AuthRepo : IAuthRepo
{
    private readonly AppDbContext _context;

    public AuthRepo(AppDbContext context)
    {
        _context = context;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> CheckLoginDetails(LoginRequest loginRequest)
    {
        try
        {
            var query = _context.Userauthentications.Where(u => u.EmailId == loginRequest.EmailId && u.Iscontinued == true);
            var userData = query.FirstOrDefault();

            if (userData != null)
            {
                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = "Email Found In Database";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Email Not Found In Database";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> CheckForgotPassEmail(string emailId)
    {
        try
        {
            var query = _context.Userauthentications.Where(u => u.EmailId == emailId && u.Iscontinued == true);
            var userData = query.FirstOrDefault();

            if (userData != null)
            {
                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = "Email Found In Database";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Email Not Found In Database";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<bool> IsSaveResetToken(string emailId, string token)
    {
        var query = _context.Userauthentications.Where(u => u.EmailId == emailId && u.Iscontinued == true);
        var userData = query.FirstOrDefault();

        if (userData != null)
        {
            ResetPasswordToken newTokenRow = new ResetPasswordToken();
            newTokenRow.ResetToken = token;
            newTokenRow.UserId = userData.UserId;
            newTokenRow.CreateAt = DateTime.Now;
            newTokenRow.IsContinue = true;
            _context.Add(newTokenRow);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
    public async Task<ResponseResult> CheckResetToken(UpdatePassword updatePassword)
    {
        try
        {
            var query = _context.ResetPasswordTokens.Where(rpt => rpt.ResetToken == updatePassword.token);
            var userData = query.FirstOrDefault();
            if (userData != null)
            {
                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = "Token Found";
            }
            else
            {
                result.Status = ResponseStatus.NotFound;
                result.Message = "Token Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        return result;
    }
    public async Task<bool> UpdatePassword(UpdatePassword updatePassword)
    {
        var query = _context.ResetPasswordTokens
                            .Include(u => u.User) // Assuming 'User' is the navigation property for UserAuthentication
                            .Where(u => u.ResetToken == updatePassword.token && u.IsContinue == true);

        var userData = query.FirstOrDefault();

        if (userData != null)
        {
            userData.User.PasswordHash = updatePassword.Password;
            userData.User.Modifyat = DateTime.Now;
            _context.Update(userData);
            _context.SaveChanges();
            return true;
        }
        return false;
    }

}
