using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using LicitAR.Core.Data.Models;
using LicitAR.Core.DI;
using LicitAR.Core.Utils;
using LicitAR.FileStorage.DI;
using Microsoft.Data.SqlClient; // Asegúrate de tener este using

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddContextRegistrations(builder.Configuration);
        builder.Services.AddIdentityRegistrations(builder.Configuration);
        builder.Services.AddAppBusinessRegistrations(builder.Configuration);
        builder.Services.AddServicesRegistrations(builder.Configuration);
        builder.Services.AddFileStorageRegistrations(builder.Configuration);

        // Add services to the container.
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AuthorizeFilter());
        });

        // Add session services
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // Middleware global para capturar errores de conexión a la base de datos
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Cannot open server") || ex.Message.Contains("Client with IP address"))
                {
                    context.Response.Redirect("/Error/ConnectionError");
                    return;
                }
                throw;
            }
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Shared/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Use session BEFORE authentication/authorization
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
