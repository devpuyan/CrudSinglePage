using Microsoft.EntityFrameworkCore;
using SinglePage.ApplicationServices.Contracts;
using SinglePage.ApplicationServices.Services;
using SinglePage.Models;
using SinglePage.Models.Services.Contracts;
using SinglePage.Models.Services.Repositories;

using Microsoft.EntityFrameworkCore;
using SinglePage.ApplicationServices.Contracts;
using SinglePage.ApplicationServices.Services;
using SinglePage.Models;
using SinglePage.Models.Services.Contracts;
using SinglePage.Models.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

#region [- AddDbContext() -]
var connectionString = builder.Configuration.GetConnectionString("FinalProjectDb") 
    ?? throw new InvalidOperationException("Connection string 'FinalProjectDb' not found.");

builder.Services.AddDbContext<ProjectDbContext>(options => 
    options.UseSqlServer(connectionString)); 
#endregion

#region [- Models IOC -]
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
#endregion

#region [- ApplicationServices IOC -]
builder.Services.AddScoped<IPersonService, PersonService>();
#endregion


var app = builder.Build();
app.UseDeveloperExceptionPage();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll"); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();