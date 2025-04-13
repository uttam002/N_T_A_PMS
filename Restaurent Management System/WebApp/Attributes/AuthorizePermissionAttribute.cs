using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMSServices.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PMSWebApp.Attributes
{
    public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly string _permissionType; // "can_view", "can_createandedit", "can_delete"

        public AuthorizePermissionAttribute(string moduleName, string permissionType)
        {
            _moduleName = moduleName;
            _permissionType = permissionType;
        }
        public void  OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated )
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

  
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            int roleIdClaim = MapRoleIdToRoleName(roleClaim);
            if ( string.IsNullOrEmpty(roleClaim)|| roleIdClaim == 0)
            {
                context.Result = new ForbidResult(); // 403 Forbidden
                return;
            }
            var permissionService = context.HttpContext.RequestServices.GetService<IRoleService>();
            if (permissionService == null || !permissionService.HasPermission(roleIdClaim, _moduleName, _permissionType).Result)
            {
                context.Result = new ForbidResult(); // 403 Forbidden
            }
        }
        private int MapRoleIdToRoleName(string role)
    {

        return role switch
        {
            "admin" => 1,
            "chef" => 2,
            "accountManager" => 3,
            _=> 0 // Default role
        };
    }
    }

}
