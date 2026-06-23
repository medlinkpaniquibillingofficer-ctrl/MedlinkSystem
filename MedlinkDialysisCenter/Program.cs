using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ALL services go BEFORE builder.Build()
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>() 
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // auto logout after 8 hours
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<HepaTestService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<VaccineService>();
builder.Services.AddScoped<IPhConsumptionService, PhConsumptionService>();

var app = builder.Build(); 

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdmin(scope.ServiceProvider);
}

app.Run();