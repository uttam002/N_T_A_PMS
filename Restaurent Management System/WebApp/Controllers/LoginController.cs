using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;

namespace PMSWebApp.Controllers;

public class LoginController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _bllAuthService;
    private readonly IJWTService _jwtService;
    private readonly IUserService _userService;

    public LoginController(IConfiguration configuration, IAuthService bllAuthService, IUserService userService, IJWTService jwtService)
    {
        _configuration = configuration;
        _bllAuthService = bllAuthService;
        _userService = userService;
        _jwtService = jwtService;

    }
    ResponseResult result = new ResponseResult();

    // [Authorize]
    public async Task<IActionResult> Index()
    {
        string userToken = Request.Cookies["refreshToken"]?? string.Empty;
        if (!string.IsNullOrEmpty(userToken))
        {
            Userauthentication user = await _jwtService.ValidateRefreshToken(userToken);
            string accessToken = await _jwtService.GenerateAccessToken(user.EmailId);
             Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });
            HttpContext.Session.SetString("Email", user.EmailId);
            HttpContext.Session.SetString("UserName", user.UserName);
            return RedirectToAction("Index", "Home");
        }

        TempData["LayoutName"] = "_LoginLayout";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        try
        {
            result = await _bllAuthService.LoginUser(loginRequest);
            Userauthentication user = (Userauthentication)result.Data;
            if (result.Status == ResponseStatus.Success)
            {
                string accessToken = await _jwtService.GenerateAccessToken(loginRequest.EmailId);
                if (loginRequest.IsRememberMe)
                {
                    string refreshToken = await _jwtService.GenerateRefreshToken();
                    await _jwtService.SaveRefreshToken(user.UserId, refreshToken);
                    Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(30)
                    });
                }
                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });
                HttpContext.Session.SetString("Email", user.EmailId);
                HttpContext.Session.SetString("UserName", user.UserName);
                TempData["ToastMessage"] = result.Message;
                TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("Index", "Login");
    }
    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out string? refreshToken) || string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token found");
        }
        Userauthentication user = await _jwtService.ValidateRefreshToken(refreshToken);
        if (user == null)
        {
            return Unauthorized("User not found");
        }
        string newAccessToken = await _jwtService.GenerateAccessToken(user.EmailId);
        Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(15) });

        return Ok(new { message = "Token refreshed" });
    }
    public IActionResult ForgotPassword()
    {
        TempData["LayoutName"] = "_LoginLayout";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> SendEmailLink(string email)
    {
        try
        {
            result = await _bllAuthService.SendForgotPassLink(email);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        MethodInfo data = (MethodInfo)result.Data;
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction(data.Method, data.Controller);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(UpdatePassword updatePassword)
    {
        try
        {
            result = await _bllAuthService.ResetPassword(updatePassword);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        MethodInfo data = (MethodInfo)result.Data;
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction(data.Method, data.Controller);
    }

}
