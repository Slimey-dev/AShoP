using AShoP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddUserManager<CustomUserManager<IdentityUser>>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Catalog}/{action=Index}/{id?}");
app.MapRazorPages();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var scope = scopeFactory.CreateScope();

var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
Task<IdentityResult> roleResult;
var email = "admin@example.com";

//Check that there is an Administrator role and create if not
var hasAdminRole = roleManager.RoleExistsAsync("Administrator");
hasAdminRole.Wait();

if (!hasAdminRole.Result)
{
    roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
    roleResult.Wait();
}

Task<IdentityUser> testUser = userManager.FindByEmailAsync(email);
testUser.Wait();

if (testUser.Result == null)
{
    var administrator = new IdentityUser();
    administrator.Email = email;
    administrator.UserName = email;

    var newUser = userManager.CreateAsync(administrator, "Password123!");
    newUser.Wait();

    if (newUser.Result.Succeeded)
    {
        var newUserRole = userManager.AddToRoleAsync(administrator, "Administrator");
        newUserRole.Wait();
    }
}

app.Run();