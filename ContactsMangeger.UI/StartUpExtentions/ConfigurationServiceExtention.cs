using ContactsMangeger.Core.Domain.IdentityEntities;
using ContactsMangeger.Core.Domain.RepositoryContracts;
using CRUDapp.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using ServiceContracts;
using Services;

namespace CRUDapp.StartUpExtentions
{
    public static class ConfigurationServiceExtention
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>();
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global", 2));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            services.AddScoped<ICountryService, CountriesService>();
            services.AddScoped<IPersonService, PersonService>();

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.Password.RequiredLength = 5; 
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 3;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>().AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>().AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context => {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });
            services.AddHttpLogging(c => c.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders);
            return services;
        }
    }
}
