using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BookDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BookRealProjectConnectionString")));
builder.Services.AddIdentity<Account , IdentityRole>().AddRoles<IdentityRole>().AddEntityFrameworkStores<BookDBContext>().AddDefaultTokenProviders(); //if have error it should be here
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using(var scope = app.Services.CreateScope())
{
    var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 

    var roles = new[] { "Admin", "User" };

    foreach(var role in roles)
    {
        if (!await rolemanager.RoleExistsAsync(role))
            await rolemanager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();

    var roles = new[] { "Admin", "User" };

    string email = "hshelter@gmail.com";
    string password = "ShelterH123!";
    if(await userManager.FindByEmailAsync(email) == null)
    {
        var user = new Account();
        user.UserName = email;
        user.Email = email;
        user.BirthDate = DateTime.Now.ToString();
        user.CustomerName = "Admin";
        user.CustomerSurname = "HShelter";
        user.Gender = "AdminGender *0*";
        await userManager.CreateAsync(user , password);

        await userManager.AddToRoleAsync(user, "Admin");
    }
    
}

app.Run();

