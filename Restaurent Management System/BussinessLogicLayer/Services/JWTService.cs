using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class JWTService : IJWTService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepo _userRepo;
    private readonly IRefreshTokenRepo _refreshTokenRepo;

    public JWTService(IConfiguration configuration, IUserRepo userRepo, IRefreshTokenRepo refreshTokenRepo)
    {
        _configuration = configuration;
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<string> GenerateAccessToken(string email)
    {
        Userauthentication user = await _userRepo.GetUserDetailsByEmailAsync(email) ?? throw new Exception("User not found");
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"] ?? ""));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        string roleName = MapRoleIdToRoleName(user.RoleId);
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.EmailId),
            new Claim(ClaimTypes.Role, roleName)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private string MapRoleIdToRoleName(int roleId)
    {

        return roleId switch
        {
            1 => "admin",
            2 => "chef",
            3 => "accountManager",
            _ => "Guest" // Default role
        };
    }
    public async Task<string> GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<Userauthentication> ValidateRefreshToken(string refreshToken)
    {
        RefreshToken storedToken = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();

        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new Exception("Invalid refresh token");
        }
        Userauthentication user = await _userRepo.GetUserAuthenticationAsync(storedToken.UserId) ?? throw new Exception("User not found");
        return user;
    }

    public async Task SaveRefreshToken(int userId, string refreshToken)
    {
        await _refreshTokenRepo.SaveRefreshToken(userId, refreshToken);
    }

    public async Task RevokeRefreshToken(string refreshToken)
    {
        RefreshToken token = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();
        if (token != null)
        {
            // (discard) Task _
            _ = _refreshTokenRepo.RemoveRefreshToken(token);
        }
    }

    public async Task<(string, string)> RefreshTokenAsync(string refreshToken)
    {
        RefreshToken token = await _refreshTokenRepo.GetRefreshToken(refreshToken) ?? new RefreshToken();
        if (token == null || token.ExpiryDate < DateTime.UtcNow)
        {
            return ("", "");
        }
        Userauthentication user = await _userRepo.GetUserAuthenticationAsync(token.UserId) ?? throw new Exception("User not found");
        string newRefreshToken = await GenerateRefreshToken();
        string newAccessToken = await GenerateAccessToken(user.EmailId);
        token.Token = newRefreshToken;
        token.ExpiryDate = DateTime.UtcNow.AddDays(30); // Set new expiry
        await _refreshTokenRepo.UpdateRefreshToken(token);

        return new(newAccessToken, newRefreshToken);

    }
}
