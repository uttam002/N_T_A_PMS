using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IAuthRepo
{
    Task<ResponseResult> CheckLoginDetails(LoginRequest loginRequest);
    Task<bool> IsSaveResetToken(string emailId, string token);
    Task<ResponseResult> CheckForgotPassEmail(string emailId);
    Task<ResponseResult> CheckResetToken(UpdatePassword updatePassword);
    Task<bool> UpdatePassword(UpdatePassword updatePassword);
}
