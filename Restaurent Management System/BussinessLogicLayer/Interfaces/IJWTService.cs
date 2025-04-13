using PMSCore.ViewModel;
using PMSData;

namespace PMSServices.Interfaces;

public interface IJWTService
{
    
    Task<string> GenerateAccessToken(string email);
    Task<string> GenerateRefreshToken();
    Task<Userauthentication> ValidateRefreshToken(string refreshToken);
    Task SaveRefreshToken(int userId, string refreshToken);
    Task RevokeRefreshToken(string refreshToken);
    Task<(string,string)> RefreshTokenAsync(string refreshToken);
}
