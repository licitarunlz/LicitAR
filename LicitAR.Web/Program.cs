using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Localization;
using LicitAR.Core.DI;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.Core.Services;
using LicitAR.FileStorage.DI;
using LicitAR.Web.Services;
using System.Globalization;

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
        
        // Envío de mails con vistas
        builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
        builder.Services.AddScoped<EmailSenderService>();
        builder.Services.AddScoped<ILicitacionNotificationService, LicitacionNotificationService>();

        builder.Services.AddHttpContextAccessor();
        
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

        // Configuración de cultura global
        var defaultCulture = new CultureInfo("es-AR");
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture),
            SupportedCultures = new[] { defaultCulture },
            SupportedUICultures = new[] { defaultCulture }
        };

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

        // Usa la configuración de localización antes de cualquier middleware que procese requests
        app.UseRequestLocalization(localizationOptions);

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
