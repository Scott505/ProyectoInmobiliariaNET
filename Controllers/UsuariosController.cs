using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace _Net.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository repository;
        private readonly IWebHostEnvironment env;

        public UsuariosController(IConfiguration config, IWebHostEnvironment env)
        {
            repository = new UsuariosRepository(config);
            this.env = env;
        }

        #region LOGIN / LOGOUT

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email y contraseña son obligatorios.");
                return View();
            }

            var usuario = repository.ObtenerPorEmail(email);
            if (usuario == null || usuario.Password != password)
            {
                ModelState.AddModelError("", "Email o contraseña incorrecta");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? usuario.Email),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            if (!string.IsNullOrEmpty(usuario.Avatar))
                claims.Add(new Claim("avatar", usuario.Avatar));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4) });

            TempData["Mensaje"] = $"Bienvenido {usuario.Nombre ?? usuario.Email}";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region INDEX / CRUD

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Index() => View(repository.ObtenerTodos());

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Create() => View(new Usuario());

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario, string Password, IFormFile? avatar)
        {
            if (!ModelState.IsValid) return View(usuario);
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "La contraseña es obligatoria.");
                return View(usuario);
            }

            usuario.Password = Password;
            usuario.Activo = true;

            try
            {
                usuario.Avatar = GuardarAvatar(avatar);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            repository.Alta(usuario);
            TempData["Mensaje"] = "Usuario creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (rol != "Administrador" && currentId != id) return RedirectToAction("Edit", new { id = currentId });

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string nombre, string email, string rol, bool activo,
                                  string nuevoPassword, IFormFile? avatar, bool borrarAvatar = false)
        {
            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole != "Administrador" && currentId != id)
                return RedirectToAction("Edit", new { id = currentId });

            // Campos editables
            usuario.Nombre = nombre;
            usuario.Email = email;
            if (userRole == "Administrador")
            {
                usuario.Rol = rol;
                usuario.Activo = activo;
            }

            if (!string.IsNullOrWhiteSpace(nuevoPassword))
                usuario.Password = nuevoPassword;

            try
            {
                usuario.Avatar = GuardarAvatar(avatar, borrarAvatar ? null : usuario.Avatar);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            repository.Modificar(usuario);

            TempData["Mensaje"] = userRole == "Administrador" ? "Usuario actualizado correctamente" : "Perfil actualizado correctamente";
            return userRole == "Administrador" ? RedirectToAction("Index") : RedirectToAction("Edit", new { id });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (rol != "Administrador" && currentId != id) return Forbid();

            return View(usuario);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = repository.ObtenerPorId(id);
            if (usuario != null && !string.IsNullOrEmpty(usuario.Avatar))
                TryDeleteAvatarFile(usuario.Avatar);

            repository.BajaLogica(id);
            TempData["Mensaje"] = "Usuario inhabilitado correctamente";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region AVATAR

        private string? GuardarAvatar(IFormFile? avatar, string? avatarPrevio = null)
        {
            if (avatar == null || avatar.Length == 0) return avatarPrevio;

            var accepted = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif" };
            if (!accepted.Contains(avatar.ContentType)) throw new InvalidOperationException("Formato de imagen no permitido.");
            if (avatar.Length > 2 * 1024 * 1024) throw new InvalidOperationException("El avatar supera 2 MB.");

            if (!string.IsNullOrEmpty(avatarPrevio)) TryDeleteAvatarFile(avatarPrevio);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            var avatarsFolder = Path.Combine(env.WebRootPath, "avatars");
            if (!Directory.Exists(avatarsFolder)) Directory.CreateDirectory(avatarsFolder);

            var filePath = Path.Combine(avatarsFolder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            avatar.CopyTo(stream);

            return "/avatars/" + fileName;
        }

        private void TryDeleteAvatarFile(string avatarPath)
        {
            if (string.IsNullOrEmpty(avatarPath)) return;
            var relative = avatarPath.TrimStart('/');
            var fullPath = Path.Combine(env.WebRootPath, relative);
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }

        #endregion
    }
}
