using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PMSServices.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PMSWebApp.Extensions
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                string token = context.Request.Cookies["accessToken"] ?? "";
                string refreshToken = context.Request.Cookies["refreshToken"] ?? "";

                if (!string.IsNullOrEmpty(token))
                {
                    ClaimsPrincipal principal = ValidateToken(token);

                    // If token is invalid, attempt refresh
                    if (principal == null && !string.IsNullOrEmpty(refreshToken))
                    {
                        IJWTService jwtService = context.RequestServices.GetRequiredService<IJWTService>();
                        (string newrRfreshToken, string newAccessToken) = await jwtService.RefreshTokenAsync(refreshToken);

                        if (newrRfreshToken != null && newAccessToken != null)
                        {
                            token = newAccessToken;

                            context.Response.Cookies.Append("accessToken", token, new CookieOptions { HttpOnly = true, Secure = true });
                            context.Response.Cookies.Append("refreshToken", newrRfreshToken, new CookieOptions { HttpOnly = true, Secure = true });

                            principal = ValidateToken(token);
                        }
                    }

                    if (principal != null)
                    {
                        context.User = principal;
                    }
                }
            }
            catch
            {
                context.Response.Cookies.Delete("accessToken");
                context.Response.Cookies.Delete("refreshToken");
                context.Response.Redirect("/Error");
                return;
            }

            await _next(context);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_config["JwtConfig:Key"] ?? "");

            try
            {
                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = false,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Strict expiration check
                };

                // Validate the token and return ClaimsPrincipal
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null ?? new ClaimsPrincipal(); // Return null if token is invalid or expired
            }
        }
    }
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
        }
    }
}
