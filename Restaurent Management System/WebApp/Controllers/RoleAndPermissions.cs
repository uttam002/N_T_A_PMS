using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSWebApp.Controllers;

public class RoleAndPermissions : Controller
{
    private readonly IRoleService _roleService;

    public RoleAndPermissions(IRoleService roleService)
    {
        _roleService = roleService;
    }

    ResponseResult result = new ResponseResult();
    public IActionResult Role()
    {
        @TempData["LayoutName"] = "_Layout";
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Permissions(string roleName)
    {

        ViewBag.RoleName = roleName;
        try
        {
            result = await _roleService.GetPermissionList(roleName);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        var permissionList = (List<PermissionDetails>)result.Data;
        @TempData["ToastMessage"] = result.Message;
        @TempData["ToastStatus"] = result.Status.ToString();
        @TempData["LayoutName"] = "_Layout";
        return View(permissionList);
    }

    public async Task<IActionResult> UpdatePermissions(List<PermissionDetails> Permissions){

        try{
           result =await _roleService.UpdatePermissions(Permissions) ;
        }catch(Exception ex){
            result.Message = ex.Message ;
            result.Status = ResponseStatus.Error;
        }

        var data = (MethodInfo) result.Data;
        @TempData["ToastMessage"] = result.Message;
        @TempData["ToastStatus"] = result.Status.ToString();
        return RedirectToAction(data.Method,data.Controller);
    }
}
