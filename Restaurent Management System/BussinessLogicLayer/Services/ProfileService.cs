using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class ProfileService : IProfileService
{

    private readonly IUserRepo _userRepo;
    private readonly ICommonServices _commonServices;
    public ProfileService(IUserRepo userRepo, ICommonServices commonServices)
    {
        _commonServices = commonServices;
        _userRepo = userRepo;
    }
    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> GetProfileAsync(string emailId)
    {
        try
        {
            result = await _userRepo.GetUserdetailsForProfileAsync(emailId);
            Userdetail userData = (Userdetail)result.Data;
            if (userData != null)
            {
                UserProfileVM userProfile = new UserProfileVM
                {
                    EmailId = userData.User.EmailId,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    UserName = userData.User.UserName,
                    RoleName = userData.User.Role.RoleName,
                    PhoneNumber = userData.PhoneNumber,
                    Address = userData.Address,
                    CityId = userData.CityId,
                    StateId = userData.StateId,
                    ContryId = userData.ContryId,
                    ZipCode = userData.ZipCode,
                    // Photo = userData.Photo,
                };
                result.Data = userProfile;
                result.Status = ResponseStatus.Success;
                result.Message = "Profile Found";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Profile Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> UpdateProfileAsync(UserProfileVM profile)
    {
        try
        {
            Userauthentication userData = await _userRepo.GetUserDetailsByEmailAsync(profile.EmailId) ?? new Userauthentication();

            if (userData != null)
            {
                if (userData.Userdetail != null)
                {
                    userData.Userdetail.FirstName = profile.FirstName;
                    userData.Userdetail.LastName = profile.LastName;
                    if (userData.Userdetail.User != null)
                    {
                        userData.UserName = profile.UserName;
                        userData.Modifyat = DateTime.Now;
                    }
                    userData.Userdetail.PhoneNumber = profile.PhoneNumber;
                    userData.Userdetail.Address = profile.Address;
                    userData.Userdetail.CityId = profile.CityId;
                    userData.Userdetail.StateId = profile.StateId;
                    userData.Userdetail.ContryId = profile.ContryId;
                    userData.Userdetail.ZipCode = profile.ZipCode;
                    if(profile.Photo != null)
                    {
                        userData.Userdetail.Photo = ConvertImageToByteArray(profile.Photo);
                    }
                    else
                    {
                        userData.Userdetail.Photo = userData.Userdetail.Photo;
                    }
                    userData.Userdetail.Modifyat = DateTime.Now;
                    userData.Userdetail.ModifyBy = profile.editorId;

                    result = await _userRepo.UpdateUserAuthenticationAsync(userData);
                }
            }
            else
            {
                result.Message = "User Details Not Found";
                result.Status = ResponseStatus.Error;
                return result;
            }
            if (result.Status == ResponseStatus.Success)
            {
                result.Message = "Profile Updated Successfully";
            }
            else
            {
                result.Message = "Error While Updating Profile";
                result.Status = ResponseStatus.Error;
            }
        }
        catch
        {
            result.Message = "Error While Updating Profile";
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> ChangePassword(ChangePasswordVM changePasswordVM)
    {
        try
        {

            changePasswordVM.OldPassword = await _commonServices.Encrypt(changePasswordVM.OldPassword);
            changePasswordVM.NewPassword = await _commonServices.Encrypt(changePasswordVM.NewPassword);

            Userauthentication userData = await _userRepo.GetUserByEmail(changePasswordVM.Email);
            if (userData.PasswordHash == changePasswordVM.OldPassword)
            {
                userData.PasswordHash = changePasswordVM.NewPassword;
                userData.Modifyat = DateTime.Now;
                result = await _userRepo.UpdateUserAuthenticationAsync(userData);
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = "Password Changed Successfully";
                }
            }
            else
            {
                result.Message = "Old Password Not Matched";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    
    //Helper Methods
    public static byte[] ConvertImageToByteArray(IFormFile file)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

}
