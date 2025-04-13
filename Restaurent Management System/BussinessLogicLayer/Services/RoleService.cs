using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSServices.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepo _roleRepo;
    private readonly IUserRepo _userRepo;

    public RoleService(IRoleRepo roleRepo,IUserRepo userRepo)
    {
        _roleRepo = roleRepo;
        _userRepo = userRepo;
    }

    ResponseResult result = new ResponseResult();

    public async Task<ResponseResult> GetPermissionList(string roleName)
    {
        try
        {
            result = await _roleRepo.GetPermissionListAsync(roleName);
            // if(result.Status == ResponseStatus.Success){

            // }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }

    public async Task<ResponseResult> UpdatePermissions(List<PermissionDetails> permissionDetailsList){
        try{
            foreach(var permission in permissionDetailsList){
                result = await _roleRepo.UpdatePermission(permission);
                if(result.Status == ResponseStatus.Error){
                    result.Message = "Failed to Update" + permission.ModuleName;
                    result.Status = ResponseStatus.NotFound;
                    break;
                }
            }   
        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        result.Data = new MethodInfo (){Method = "Role",Controller = "RoleAndPermissions"};
        return result;
    }

    public async Task<bool> HasPermission(int roleId, string moduleName, string permissionType)
    {
        return await _roleRepo.UserHasPermission(roleId, moduleName,  permissionType);
    }
}
