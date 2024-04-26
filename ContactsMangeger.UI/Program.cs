using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using ServiceContracts;
using Services;
using Serilog;
using CRUDapp.Filters.ActionFilters;
using CRUDapp.StartUpExtentions;
using CRUDapp.MiddleWares;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//    loggingProvider.AddDebug();
//});
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services);
});
builder.Services.ConfigureServices(builder.Configuration);
var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseHttpLogging();
if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}
//app.Logger.LogDebug("debug message");
//app.Logger.LogInformation("info message");
//app.Logger.LogWarning("warning message");
//app.Logger.LogError("error message");
//app.Logger.LogCritical("critical message");
//app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name:"areas",
        pattern:"{area:exists}/{controller=Home}/{action=Index}"
        );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}"
        );
});
app.Run();
public partial class Program { } // make auto-generated Program class accessable programmatically