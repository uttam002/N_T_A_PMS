using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface IRoleRepo
{
    Task<ResponseResult> GetPermissionListAsync(string roleName);
    Task<ResponseResult> UpdatePermission(PermissionDetails permissionDetails);
    Task<bool> UserHasPermission(int roleId, string moduleName, string permissionType);
}

