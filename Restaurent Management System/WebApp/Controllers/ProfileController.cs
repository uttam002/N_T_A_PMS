using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;

namespace PMSWebApp.Controllers;

public class ProfileController : Controller
{

    private readonly IProfileService _profileService;
    private readonly ICommonServices _commonServices;
    private readonly IUserService _userService;
    private readonly IJWTService _jwtService;
    public ProfileController(IProfileService profileService, ICommonServices commonServices, IUserService userService, IJWTService jwtService)
    {
        _profileService = profileService;
        _commonServices = commonServices;
        _userService = userService;
        _jwtService = jwtService;
    }
    ResponseResult result = new ResponseResult();

    public async Task<IActionResult> UserProfile()
    {
        try
        {
            string email = HttpContext.Session.GetString("Email")?? string.Empty;
            if (email != null)
            {
                result = await _profileService.GetProfileAsync(email);
                UserProfileVM data = (UserProfileVM)result.Data;
                var countryData = await _commonServices.GetCountryList();
                List<ContryList> countryList = countryData.Data as List<ContryList>??new List<ContryList>();
                ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
                TempData["LayoutName"] = "_Layout";
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Login");
        }
    }
    public IActionResult ChangePassword()
    {
        TempData["LayoutName"] = "_Layout";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
    {
        try
        {
            result = await _profileService.ChangePassword(changePasswordVM);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return View();
    }

    public async Task<IActionResult> UpdateProfile(UserProfileVM profile)
    {
        try
        {
            if (ModelState.IsValid)
            {
                result = await _profileService.UpdateProfileAsync(profile);
            }
            else
            {
                result.Message = "Invalid Data";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("Index", "Home");
    }
    public IActionResult Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            _jwtService.RevokeRefreshToken(refreshToken);
        }

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        HttpContext.Session.Remove("Email");
        HttpContext.Session.Remove("UserName");
        TempData["ToastMessage"] = "Logout Successfully";
        TempData["ToastStatus"] = ResponseStatus.Success.ToString(); // Convert Enum to String
        return RedirectToAction("Index", "Login");
    }

    // Helpers
    public async Task<IActionResult> GetStates(int countryId)
    {
        var stateList = await _commonServices.GetStateList(countryId); // Await the method call if it's async

        if (stateList.Data != null)
        {
            return Json(stateList.Data); // Return state list in JSON format
        }

        return Json(new { success = false, message = "No states found." }); // Return a failure response
    }

    public async Task<IActionResult> GetCities(int stateId)
    {
        var cityList = await _commonServices.GetCityList(stateId); // Await the method call if it's async

        if (cityList.Data != null)
        {
            return Json(cityList.Data); // Return city list in JSON format
        }

        return Json(new { success = false, message = "No cities found." }); // Return a failure response
    }

    public async Task<IActionResult> GetUserPhoto(string email)
    {
        byte[] UserImg = await _userService.GetUserProfileImgByEmailAsByteStream(email);
        return File(UserImg, "image/jpeg");  // Return image as a file
    }
}
