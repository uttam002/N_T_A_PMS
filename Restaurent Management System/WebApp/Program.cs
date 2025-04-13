using PMSWebApp.Extensions;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices(configuration);
builder.Services.AddSignalR();

var app = builder.Build();

// Configure middleware and error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<SiganlRHUB>("/signalrhub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.Run();
