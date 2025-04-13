using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSServices.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepo _dalAuthService;
    private readonly ICommonServices _commonServices;
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;
    // private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthService(IAuthRepo dALAuthService, ICommonServices commonServices, IConfiguration configuration, IUserRepo userRepo)
    {
        _dalAuthService = dALAuthService;
        _commonServices = commonServices;
        _configuration = configuration;
        _userRepo = userRepo;
        // , IHttpContextAccessor httpContextAccessor
        // _httpContextAccessor = httpContextAccessor;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> LoginUser(LoginRequest loginRequest)
    {
        try
        {
            result = await _dalAuthService.CheckLoginDetails(loginRequest);

            if (result.Message == "Email Found In Database")
            {
                Userauthentication usrObj = (Userauthentication)result.Data;
                if (usrObj.PasswordHash == await _commonServices.Encrypt(loginRequest.Password))
                {
                    result.Message = "Login Successfully";
                }
                else
                {
                    result.Message = "Password Is Invalid";
                    result.Status = ResponseStatus.Error;
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
    public async Task<ResponseResult> SendForgotPassLink(string EmailId)
    {
        try
        {
            result = await _dalAuthService.CheckForgotPassEmail(EmailId);

            if (result.Message == "Email Found In Database")
            {
                if (await HelperToSend(EmailId))
                {
                    result.Data = new MethodInfo { Method = "Index", Controller = "Login" };
                    result.Message = "Check Mail For Reset Password Link";
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = "Issue to send Email PLease Try Again";
                    result.Status = ResponseStatus.Error;
                }
            }
            else
            {
                result.Data = new MethodInfo { Method = "ForgotPassword", Controller = "Login" };
                result.Message = "No User Found With This Email";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            result.Data = new MethodInfo { Method = "ForgotPassword", Controller = "Login" };
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> ResetPassword(UpdatePassword updatePassword)
    {
        result = await _dalAuthService.CheckResetToken(updatePassword);

        if (result.Data != null)
        {
            ResetPasswordToken tokenAvail = (ResetPasswordToken)result.Data;
            TimeSpan linkLifeTime = TimeOnly.FromDateTime(DateTime.Now) - TimeOnly.FromDateTime(tokenAvail.CreateAt);
            if (linkLifeTime.Minutes < 30)
            {
                if (updatePassword.Password == updatePassword.confirmPassword)
                {
                    updatePassword.Password = await _commonServices.Encrypt(updatePassword.Password);
                    if (await _dalAuthService.UpdatePassword(updatePassword))
                    {
                        // result.Data = new MethodInfo { Method = "Index", Controller = "Login" };
                        result.Message = "Password Updated Successfully";
                        result.Status = ResponseStatus.Success;
                    }
                    else
                    {
                        result.Message = "Issue to Update Password";
                        result.Status = ResponseStatus.Error;
                    }
                }
                else
                {
                    result.Message = "Password And Confirm Password Not Match";
                    result.Status = ResponseStatus.Error;
                }

            }
            else
            {
                result.Message = "Reset Password Link Expired";
                result.Status = ResponseStatus.Error;
            }
        }
        result.Data = new MethodInfo { Method = "Index", Controller = "Login" };

        return result;
    }
    private async Task<bool> HelperToSend(string emailId)
    {
        string token = await GenerateResetToken(emailId);
        string resetLink = $"http://localhost:5253/Login/ResetPassword?token={token}";
        string emailBody = await GetEmailBodyAsync("ForgotPasswordFormat.html");
        emailBody = emailBody.Replace("{{reset_link}}", resetLink);
        string subject = "Reset Password Link";
        return await SendEmailAsync(emailId, subject, emailBody);
    }
    private async Task<string> GenerateResetToken(string emailId)
    {
        string token;
        using (RandomNumberGenerator randomnumbergenerator = RandomNumberGenerator.Create())
        {
            byte[] tokenBytes = new byte[32];
            randomnumbergenerator.GetBytes(tokenBytes);
            token = Convert.ToBase64String(tokenBytes)
                        .Replace("+", "-") // URL-safe Base64
                        .Replace("/", "_")
                        .Replace("=", ""); // Remove padding
        }

        if (token != null)
        {
            // Save token in database
            if (await _dalAuthService.IsSaveResetToken(emailId, token))
            {
                return token;
            }
            else
            {
                return "Error to save token";
            }

        }
        return token ?? "";
    }
    public async Task<string> GetEmailBodyAsync(string templateName)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", templateName);
        return await File.ReadAllTextAsync(templatePath);
    }
    public async Task<bool> SendEmailAsync(string emailId, string subject, string emailBody)
    {
        string senderEmail = "test.dotnet@etatvasoft.com";
        string senderPassword = "P}N^{z-]7Ilp";
        string smtpServer = "mail.etatvasoft.com";
        int smtpPort = 587;

        try
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = emailBody,
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            mail.To.Add(emailId);
            await client.SendMailAsync(mail);

            return true; // Email sent successfully
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            return false; // Email failed to send
        }
    }


    // public async Task<string> DecodeJwtToken(string token)
    // {
    //     try
    //     {
    //         JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
    //         JwtSecurityToken jsonToken = handler.ReadJwtToken(token);

    //         // Extract email from token (Handles URI-encoded claims)
    //         Claim emailClaim = jsonToken.Claims.FirstOrDefault(c =>
    //             c.Type == ClaimTypes.Email ||
    //             c.Type == "email" ||
    //             c.Type.EndsWith("emailaddress") // Handles URI-style claims
    //         ) ?? new Claim(ClaimTypes.Email, string.Empty);

    //         return emailClaim.Value;
    //     }
    //     catch
    //     {
    //         return null ?? ""; // Handle invalid token gracefully
    //     }
    // }
}


