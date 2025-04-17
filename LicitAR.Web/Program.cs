using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using LicitAR.Core.Data.Models;
using Microsoft.Extensions.Configuration;
using LicitAR.Core.DI;
using LicitAR.Core.Utils;
using LicitAR.Core.Business.Identidad;
using LicitAR.FileStorage.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddContextRegistrations(builder.Configuration);

builder.Services.AddIdentityRegistrations(builder.Configuration);

builder.Services.AddAppBusinessRegistrations(builder.Configuration);


builder.Services.AddFileStorageRegistrations(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
 
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
 
 
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
 

app.Run();
