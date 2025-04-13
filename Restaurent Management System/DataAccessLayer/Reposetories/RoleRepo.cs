using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class RoleRepo : IRoleRepo
{

    private readonly AppDbContext _context;

    public RoleRepo(AppDbContext context)
    {
        _context = context;
    }
    ResponseResult result = new ResponseResult();

    public async Task<ResponseResult> GetPermissionListAsync(string roleName)
    {
        try
        {
            if (roleName != null)
            {
                var query = _context.Permissions
                                    .Include(p => p.Role)
                                    .Include(p => p.Module)
                                    .Where(p => p.Role.RoleName == roleName);
                var permissionList = query.ToList();
                if (permissionList.Count > 0)
                {
                    List<PermissionDetails> permissionDetails = new List<PermissionDetails>();
                    foreach (var permission in permissionList)
                    {
                        PermissionDetails temp = new PermissionDetails();
                        temp.PermissionId = permission.PId;
                        temp.RoleId = permission.RoleId;
                        temp.ModuleId = permission.ModuleId;
                        temp.CanCreateandedit = permission.CanCreateandedit;
                        temp.CanDelete = permission.CanDelete;
                        temp.CanView = permission.CanView;
                        temp.ModuleName = permission.Module.ModuleName;

                        permissionDetails.Add(temp);
                    }
                    result.Data = permissionDetails;
                    result.Status = ResponseStatus.Success;
                    result.Message = "Permission List Get Successfully";
                }
                else
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = "Permission List Not Found";
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> UpdatePermission(PermissionDetails permissionDetails)
    {
        try
        {
            var query = _context.Permissions.Where(x => x.PId == permissionDetails.PermissionId);
            var permission = query.FirstOrDefault();
            if (permission != null)
            {
                permission.CanDelete = permissionDetails.CanDelete;
                permission.CanView = permissionDetails.CanView;
                permission.CanCreateandedit = permissionDetails.CanCreateandedit;

                permission.Modifyat = DateTime.Now;
                permission.Modifyby = permissionDetails.EditorId;

                _context.Permissions.Update(permission);
                await _context.SaveChangesAsync();
                result.Status = ResponseStatus.Success;
                result.Message = "Permission Updated Successfully";
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = "Permission Not Found";
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<bool> UserHasPermission(int roleId, string moduleName, string permissionType)
{
    var permission = await _context.Permissions
        .Include(p => p.Module)
        .Where(p => p.RoleId == roleId && p.Module.ModuleName == moduleName)
        .FirstOrDefaultAsync();

    if (permission == null)
    {
        return false; // No permission found
    }

    return permissionType switch
    {
        "can_view" => permission.CanView,
        "can_createandedit" => permission.CanCreateandedit,
        "can_delete" => permission.CanDelete,
        _ => false
    };
}

}
