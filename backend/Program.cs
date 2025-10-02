using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.Models;
using GestorEncuestas_MVC.Services; // ✅ AÑADIR este using
using Microsoft.AspNetCore.Mvc.NewtonsoftJson; // ✅ AÑADIR este using
using Newtonsoft.Json; // ✅ AÑADIR este using

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ AÑADIR Configuración para la API de exportación Excel
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Configurar Entity Framework con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Configurar Identity
builder.Services.AddIdentity<Usuario, Rol>(options =>
{
    // Configuración de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    
    // Configuración de usuario
    options.User.RequireUniqueEmail = false;
    
    // Configuración de bloqueo
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Configuración de inicio de sesión
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar autenticación con cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccessDenied";
    options.LogoutPath = "/Cuenta/Logout";
    options.SlidingExpiration = true;
});

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireUser", policy => 
        policy.RequireRole("User", "Admin"));
});

// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: UseAuthentication debe ir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// ✅ AÑADIR Mapeo de controladores API (IMPORTANTE para que funcionen los endpoints API)
app.MapControllers();

// Crear roles iniciales y usuario admin (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Rol>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        
        // Crear roles si no existen
        var roles = new[] { 
            new { Name = "Admin", Id = 1 }, 
            new { Name = "User", Id = 2 } 
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(new Rol { 
                    Id = role.Id,
                    DisplayRolNombre = role.Name, 
                    Name = role.Name,
                    NormalizedName = role.Name.ToUpper()
                });
            }
        }
        
        // Crear usuario admin si no existe
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            var user = new Usuario
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@email.com",
                NormalizedEmail = "ADMIN@EMAIL.COM",
                EmailConfirmed = true,
                RolId = 1 // Asignar el ID del rol Admin
            };
            
            var result = await userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();