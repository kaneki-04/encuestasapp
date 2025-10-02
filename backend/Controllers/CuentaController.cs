using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using GestorEncuestas_MVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestorEncuestas_MVC.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly ILogger<CuentaController> _logger;

        public CuentaController(
            SignInManager<Usuario> signInManager,
            UserManager<Usuario> userManager,
            RoleManager<Rol> roleManager,
            ILogger<CuentaController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: /Cuenta/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Intentar iniciar sesión usando el username (DisplayUsername)
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Username} inició sesión.", model.Username);
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Encuestas");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: /Cuenta/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Cuenta/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Buscar el rol User (asumiendo que existe)
                var userRole = await _roleManager.FindByNameAsync("User");
                if (userRole == null)
                {
                    // Crear el rol User si no existe
                    userRole = new Rol { Name = "User", DisplayRolNombre = "User" };
                    var roleResult = await _roleManager.CreateAsync(userRole);
                    
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error al crear el rol de usuario.");
                        return View(model);
                    }
                }

                var user = new Usuario 
                { 
                    UserName = model.Username,
                    RolId = userRole.Id // Asignar el ID del rol User
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Asignar el rol "User" al nuevo usuario
                    await _userManager.AddToRoleAsync(user, "User");
                    
                    _logger.LogInformation("Usuario {Username} se registró exitosamente.", model.Username);

                    // Iniciar sesión automáticamente después del registro
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Encuestas");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: /Cuenta/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Cuenta/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Cuenta/Perfil
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var model = new PerfilViewModel
            {
                Username = user.UserName  ?? string.Empty
            };

            return View(model);
        }
    }
}