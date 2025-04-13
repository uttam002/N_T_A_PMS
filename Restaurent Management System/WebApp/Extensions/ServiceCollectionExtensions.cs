using Microsoft.AspNetCore.Http.Features;
using PMSData;
using PMSData.Interfaces;
using PMSData.Reposetories;
using PMSData.Repositories;
using PMSServices.Interfaces;
using PMSServices.Services;

namespace PMSWebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure file upload size
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB limit
            });

        

            // DAL (Data Access Layer)
            services.AddScoped<ICommonRepo, CommonRepo>();
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<IItemRepo, ItemRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IModifierRepo, ModifierRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRefreshTokenRepo,RefreshTokenRepo>();
            services.AddScoped<IItemModifierRepo,ItemModifierRepo>();
            services.AddScoped<ITableRepo,TableRepo>();
            services.AddScoped<ISectionRepo,SectionRepo>();
            services.AddScoped<ITaxesRepo,Taxesrepo>();
            services.AddScoped<IOrderRepo,OrderRepo>();
            services.AddScoped<IInvoiceRepo,InvoiceRepo>();
            services.AddScoped<IInvoiceItemMappingRepo,InvoiceItemMappingRepo>();
            services.AddScoped<IInvoiceTaxesMappingRepo,InvoiceTaxesMappingRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>(); 

            // BLL (Business Logic Layer)
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICommonServices, CommonServices>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IModifierService, ModifierService>();
            services.AddScoped<IItemModifierService, ItemModifierService>();
            services.AddScoped<ISectionAndTablesService, SectionAndTablesService>();
            services.AddScoped<ITaxesAndFeesService,TaxesAndFeesService>();
            services.AddScoped<IOrdersService,OrdersService>();
            services.AddScoped<ICustomerService,CustomerService>();
            services.AddScoped<IOrderAppService,OrderAppService>();

            // Add Database Context
            services.AddDbContext<AppDbContext>();

            // Add Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}
