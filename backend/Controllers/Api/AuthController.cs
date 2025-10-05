using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using GestorEncuestas_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<Usuario> signInManager,
            UserManager<Usuario> userManager,
            RoleManager<Rol> roleManager,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(
                request.Username, 
                request.Password, 
                request.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {Username} inició sesión via API.", request.Username);
                
                var user = await _userManager.FindByNameAsync(request.Username);
                var roles = await _userManager.GetRolesAsync(user);
                
                return Ok(new LoginResponse 
                { 
                    Success = true, 
                    Message = "Login exitoso",
                    User = new UserInfo 
                    { 
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles
                    }
                });
            }
            else
            {
                return Unauthorized(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Usuario o contraseña incorrectos" 
                });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterApiRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar el rol User
            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole == null)
            {
                userRole = new Rol { Name = "User", DisplayRolNombre = "User" };
                var roleResult = await _roleManager.CreateAsync(userRole);
                
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { Message = "Error al crear el rol de usuario." });
                }
            }

            var user = new Usuario 
            { 
                UserName = request.Username,
                RolId = userRole.Id
            };
            
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                
                _logger.LogInformation("Usuario {Username} se registró exitosamente via API.", request.Username);

                // Iniciar sesión automáticamente
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                var roles = await _userManager.GetRolesAsync(user);
                
                return Ok(new LoginResponse 
                { 
                    Success = true, 
                    Message = "Registro exitoso",
                    User = new UserInfo 
                    { 
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles
                    }
                });
            }

            return BadRequest(new { Message = "Error en el registro", Errors = result.Errors });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión via API.");
            return Ok(new { Message = "Logout exitoso" });
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            return Ok(new UserInfo 
            { 
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            });
        }
    }

    // DTOs para la API
    public class LoginApiRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class RegisterApiRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}