using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _appDbContext;

        public UserRepo(AppDbContext context)
        {
            _appDbContext = context;
        }
        ResponseResult result = new ResponseResult();
        // Get IMG by Email Id

        public async Task<Userauthentication?> GetUserAuthenticationAsync(int userId)
        {
            return await _appDbContext.Userauthentications.FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<ResponseResult> GetUserdetailsForProfileAsync(string emailId)
        {
            try
            {
                IQueryable<Userdetail> query = _appDbContext.Userdetails
                                    .Include(u => u.User) // UserdetailUsers
                                    .Include(u => u.User.Role)
                                    .Where(u => u.User.EmailId == emailId && u.User.Iscontinued == true);

                Userdetail userData = await query.FirstOrDefaultAsync()
                    ?? throw new Exception("User not found");

                result.Data = userData;
                result.Status = ResponseStatus.Success;
                result.Message = "User Found";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<byte[]> GetUserProfileImgByEmailAsyncAsByteStream(string email)
        {
            byte[] imgPath = await _appDbContext.Userdetails
                                            .Include(u => u.User)
                                            .Where(e => e.User.EmailId == email)
                                            .Select(i => i.Photo)
                                            .FirstOrDefaultAsync() ?? Array.Empty<byte>();
            return imgPath;
        }
        // Get User Name by email id 
        public async Task<string> GetUserNameByEmailAsync(string email)
        {
            try
            {
                var userName = await _appDbContext.Userauthentications
                                        .Where(u => u.EmailId == email)
                                        .Select(u => u.UserName)
                                        .FirstOrDefaultAsync();

                return userName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetUserNameByEmail: {ex.Message}");
                return null;
            }
        }
        // Fetch user authentication details by email
        public async Task<Userdetail?> GetUserDataAsync(string email)
        {
            try
            {
                return await _appDbContext.Userdetails
                    .Include(u => u.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(u => u.User.EmailId == email && u.User.Iscontinued == true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetUserAuthenticationAsync: {ex.Message}");
                return null;
            }
        }

        // Get user ID by email
        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            try
            {
                var userId = await _appDbContext.Userauthentications
                    .Where(u => u.EmailId == email)
                    .Select(u => (int?)u.UserId)
                    .FirstOrDefaultAsync();

                return userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetUserIdByEmailAsync: {ex.Message}");
                return null;
            }
        }

        // Get email by user ID
        public async Task<string?> GetEmailByUserIdAsync(int userId)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Where(u => u.UserId == userId)
                    .Select(u => u.EmailId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetEmailByUserIdAsync: {ex.Message}");
                return null;
            }
        }

        // Check if an email already exists in the database
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .AnyAsync(u => u.EmailId == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] IsEmailExistsAsync: {ex.Message}");
                return false;
            }
        }

        // Fetch complete user details by email
        public async Task<Userauthentication?> GetUserDetailsByEmailAsync(string email)
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Include(u => u.Userdetail)
                    .FirstOrDefaultAsync(u => u.EmailId == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetUserDetailsByEmailAsync: {ex.Message}");
                return null;
            }
        }

        // Get all users (excluding deleted users)
        public async Task<List<Userauthentication>> GetAllUsersAsync()
        {
            try
            {
                return await _appDbContext.Userauthentications
                    .Where(u => !u.Iscontinued ?? false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetAllUsersAsync: {ex.Message}");
                return new List<Userauthentication>();
            }
        }
        // Get User Details byUser d
        public async Task<Userdetail?> GetUserDetailsByUserIdAsync(int id)
        {
            return await _appDbContext.Userdetails
                                    .Include(u => u.User)
                                    .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<ResponseResult> AddNewUserAsync(Userauthentication user)
        {
            try
            {
                if (user == null)
                {
                    result.Message = "UserAuth Details is null at repository";
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                _appDbContext.Userauthentications.Add(user);
                await _appDbContext.SaveChangesAsync(); // Save to generate UserId
                result.Data = await _appDbContext.Userauthentications
                                        .Where(u => u.EmailId == user.EmailId)
                                        .Select(u => u.UserId)
                                        .FirstOrDefaultAsync();
                result.Message = "User Added Successfully";
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> AddUserDataAsync(Userdetail userData)
        {
            try
            {
                if (userData == null)
                {
                    result.Message = "UserData is null at repository";
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                _appDbContext.Userdetails.Add(userData);
                await _appDbContext.SaveChangesAsync(); // Save to generate UserId

                result.Message = "User Added Successfully";
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetUsers(PaginationDetails paginationDetails)
        {
            try
            {
                IQueryable<Userdetail> query = _appDbContext.Userdetails
                                    .Include(u => u.User.Role)
                                    .Where(u => u.Isactive == true && u.User.Iscontinued == true);

                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    query = query.Where(u => u.FirstName.ToLower().Contains(paginationDetails.SearchQuery) ||
                                             u.User.EmailId.ToLower().Contains(paginationDetails.SearchQuery) ||
                                             u.User.Role.RoleName.ToLower().Contains(paginationDetails.SearchQuery));
                }

                switch (paginationDetails.SortColumn.ToLower())
                {
                    case "name":
                        query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(e => e.FirstName) : query.OrderByDescending(e => e.FirstName);
                        break;
                    case "role":
                        query = (paginationDetails.SortOrder == "asc") ? query.OrderBy(e => e.User.Role.RoleName) : query.OrderByDescending(e => e.User.Role.RoleName);
                        break;
                    default:
                        query = query.OrderBy(e => e.FirstName); // Default sorting
                        break;
                }
                paginationDetails.totalRecords = await query.CountAsync(); // Count filtered results

                List<User> userList = await query
                                .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                .Take(paginationDetails.PageSize)
                                .Select(u => new User
                                {
                                    Id = u.UserId,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Email = u.User.EmailId,
                                    PhoneNumber = u.PhoneNumber,
                                    Role = u.User.Role.RoleName,
                                    Status = u.Status,
                                    imgData = u.Photo
                                })
                                .ToListAsync();

                result.Data = userList;
                result.Status = ResponseStatus.Success;
                result.Message = "Users Fetched Successfully";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> GetUserDetailsAsync(int userId)
        {

            try
            {
                var query = _appDbContext.Userdetails.Include(u => u.User).Where(u => u.UserId == userId);
                var userdetail = await query.FirstOrDefaultAsync();

                if (userdetail != null)
                {
                    UpdateUser user = new UpdateUser();
                    user.UserId = userdetail.UserId;
                    user.EmailId = userdetail.User.EmailId;
                    user.FirstName = userdetail.FirstName;
                    user.LastName = userdetail.LastName;
                    user.UserName = userdetail.User.UserName;
                    user.RoleId = userdetail.User.RoleId;
                    user.Status = userdetail.Status;
                    user.PhoneNumber = userdetail.PhoneNumber;
                    user.Address = userdetail.Address;
                    user.ZipCode = userdetail.ZipCode;
                    // user.Photo = userdetail.PhotoData;
                    user.ContryId = userdetail.ContryId;
                    user.StateId = userdetail.StateId;
                    user.CityId = userdetail.CityId;

                    result.Data = user;
                    result.Status = ResponseStatus.Success;
                    result.Message = "User Found successfully!!!";
                }
                else
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = "User Not Found successfully!!!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }
        public byte[] ConvertImageToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<Userauthentication> GetUserByEmail(string email)
        {
            return await _appDbContext.Userauthentications.FirstOrDefaultAsync(u => u.EmailId == email);
        }

        public async Task<ResponseResult> UpdateUserAuthenticationAsync(Userauthentication user)
        {
            try
            {
                if (user == null)
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = "User Not Found";
                }
                else
                {
                    _appDbContext.Userauthentications.Update(user);
                    await _appDbContext.SaveChangesAsync();
                    result.Status = ResponseStatus.Success;
                    result.Message = "User Update Successfully";
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateUserDetailsAsync(Userdetail user)
        {
            try
            {
                if (user == null)
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = "User Not Found";
                }
                else
                {
                    _appDbContext.Userdetails.Update(user);
                    await _appDbContext.SaveChangesAsync();
                    result.Status = ResponseStatus.Success;
                    result.Message = "User Update Successfully";
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}
