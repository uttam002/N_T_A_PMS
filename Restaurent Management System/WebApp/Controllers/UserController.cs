using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using Microsoft.AspNetCore.SignalR;
namespace PMSWebApp.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ICommonServices _commonServices;
    // private readonly IHubContext<SiganlRHUB> _hubContext;
// , IHubContext<SiganlRHUB> hubContext
    public UserController(IUserService userService, ICommonServices commonServices)
    {
        _userService = userService;
        _commonServices = commonServices;
        // _hubContext = hubContext;
    }
    ResponseResult result = new ResponseResult();
    // http://localhost:5253/User/TestSignalR
    // [HttpGet]
    // public async Task<IActionResult> TestSignalR()
    // {
    //     await _hubContext.Clients.All.SendAsync("UserDataChanged", "test", new { Message = "Hello from SignalR!" });
    //     return Ok("SignalR message sent!");
    // }
    // [AuthorizePermission("Users","can_View")]
    [HttpGet]
    public async Task<IActionResult> UserList(PaginationDetails paginationDetails)
    {
        IEnumerable<User> userList = new List<User>();
        try
        {
            result = await _userService.GetUsers(paginationDetails);
            userList = result.Data as IEnumerable<User> ?? new List<User>();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string partialView = await this.RenderPartialViewToString("_Partial_UserGrid", userList);

                return Json(new
                {
                    partialView = partialView,
                    paginationDetails = paginationDetails,
                    message = result.Message,
                    status = result.Status.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["LayoutName"] = "_Layout";
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString();

        return View((userList, paginationDetails));
    }
    public async Task<IActionResult> AddUser()
    {
        ResponseResult countryData = await _commonServices.GetCountryList();
        List<ContryList> countryList = countryData.Data as List<ContryList> ?? new List<ContryList>();
        ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
        TempData["LayoutName"] = "_Layout";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddUser(NewUser newUser)
    {
        try
        {
            if (ModelState.IsValid)
            {
                result = await _userService.AddUser(newUser);
            }
            else
            {
                result.Message = "Model State is InValid";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        // await _hubContext.Clients.Group("UserList").SendAsync("UserDataChanged", "add", newUser);
        return RedirectToAction("AddUser", "User");
    }

    [HttpGet]
    // [AuthorizePermission("Users","can_createandedit")]
    public async Task<IActionResult> EditUser(int id)
    {
        try
        {
            if (id != 0)
            {
                result = await _userService.GetUserDetails(id);
            }
            else
            {
                result.Message = "User not exit!!!";
                result.Status = ResponseStatus.NotFound;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        ResponseResult countryData = await _commonServices.GetCountryList();
        List<ContryList> countryList = countryData.Data as List<ContryList> ?? new List<ContryList>();
        ViewBag.CountryList = new SelectList(countryList, "ContryId", "ContryName");
        UpdateUser user = (UpdateUser)result.Data;
        TempData["LayoutName"] = "_Layout";
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(UpdateUser updateUser)
    {
        try
        {
            if (ModelState.IsValid)
            {
                result = await _userService.EditUser(updateUser);
            }
            else
            {
                result.Message = "Model State is InValid";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        // await _hubContext.Clients.Group("UserList").SendAsync("UserDataChanged", "update", updateUser);
        return RedirectToAction("UserList", "User");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int userId, int editor)
    {
        try
        {
            result = await _userService.DeleteUser(userId, editor);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        // await _hubContext.Clients.Group("UserPage").SendAsync("UserDataChanged", "delete", result);
        return RedirectToAction("UserList", "User");
    }

    // Helper Method
    public async Task<IActionResult> GetImage(byte[] imgData)
    {
        return File(imgData, "image/jpeg");
    }

}
