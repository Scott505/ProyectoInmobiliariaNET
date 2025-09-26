using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using _Net.Models;
using Microsoft.Extensions.Configuration;

namespace _Net.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository repository;

        public UsuariosController(IConfiguration config)
        {
            repository = new UsuariosRepository(config);
        }

        // -------------------------
        // leer rol / id (Session first, cookie fallback)
        // -------------------------
        private string? GetUserRole()
        {
            try
            {
                var rol = HttpContext.Session.GetString("UsuarioRol");
                if (!string.IsNullOrEmpty(rol)) return rol;
            }
            catch { /* session may not be configured, fallback to cookie */ }

            if (Request.Cookies.ContainsKey("UsuarioRol"))
                return Request.Cookies["UsuarioRol"];

            return null;
        }

        private int? GetUserId()
        {
            try
            {
                var id = HttpContext.Session.GetInt32("UsuarioId");
                if (id.HasValue) return id.Value;
            }
            catch { /* fallback to cookie */ }

            if (Request.Cookies.ContainsKey("UsuarioId") && int.TryParse(Request.Cookies["UsuarioId"], out var cid))
                return cid;

            return null;
        }

        private bool IsAdmin() => string.Equals(GetUserRole(), "Administrador", System.StringComparison.Ordinal);

        // -------------------------
        // LOGIN / LOGOUT
        // -------------------------
        [HttpGet]
        public IActionResult Login()
        {
            // si ya está logueado, llevar a Home
            if (IsAdmin() || GetUserId().HasValue())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
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

            // Guardar en session si está disponible
            try
            {
                HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
                HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre ?? string.Empty);
            }
            catch
            {
                // session no configurada: usar cookies como fallback (4 horas)
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Expires = DateTimeOffset.UtcNow.AddHours(4),
                    SameSite = SameSiteMode.Lax,
                    Secure = false
                };
                Response.Cookies.Append("UsuarioId", usuario.IdUsuario.ToString(), cookieOptions);
                Response.Cookies.Append("UsuarioRol", usuario.Rol ?? "Empleado", cookieOptions);
                Response.Cookies.Append("UsuarioNombre", usuario.Nombre ?? string.Empty, cookieOptions);
            }

            TempData["Mensaje"] = $"Bienvenido {usuario.Nombre ?? usuario.Email}";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
            }
            catch { /* ignore if session not configured */ }

            // Borrar cookies fallback si quedaron
            Response.Cookies.Delete("UsuarioId");
            Response.Cookies.Delete("UsuarioRol");
            Response.Cookies.Delete("UsuarioNombre");

            return RedirectToAction(nameof(Login));
        }

        // -------------------------
        // INDEX (listado) - SOLO ADMIN
        // -------------------------
        public IActionResult Index()
        {
            if (!IsAdmin()) return Forbid();

            var usuarios = repository.ObtenerTodos();
            return View(usuarios);
        }

        // -------------------------
        // CREATE- SOLO ADMIN
        // -------------------------
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return Forbid();
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario, string Password)
        {
            if (!IsAdmin()) return Forbid();

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "La contraseña es obligatoria.");
                return View(usuario);
            }

            usuario.Password = Password;
            usuario.Activo = true;

            repository.Alta(usuario);
            TempData["Mensaje"] = "Usuario creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------
        // EDIT
        // - Admin puede editar cualquiera
        // - Empleado sólo su propio perfil
        // -------------------------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var currentId = GetUserId();
            var rol = GetUserRole();

            // Empleado solo puede editar su propio perfil
            if (rol != "Administrador" && currentId != id)
                return RedirectToAction("Edit", new { id = currentId });

            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string nombre, string email, string rol, bool activo, string NuevoPassword)
        {
            var currentId = GetUserId();
            var userRole = GetUserRole();

            var usuario = repository.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            // Empleado solo puede modificar su nombre, email y contra
            if (userRole != "Administrador")
            {
                if (currentId != id)
                    return RedirectToAction("Edit", new { id = currentId });

                usuario.Nombre = nombre;
                usuario.Email = email;
                if (!string.IsNullOrWhiteSpace(NuevoPassword))
                    usuario.Password = NuevoPassword;

                repository.Modificar(usuario);
                TempData["Mensaje"] = "Perfil actualizado correctamente";

                // Vuelve a su propio Edit
                return RedirectToAction("Edit", new { id = id });
            }

            // Administrador: puede modificar todo
            usuario.Nombre = nombre;
            usuario.Email = email;
            usuario.Rol = rol;
            usuario.Activo = activo;
            if (!string.IsNullOrWhiteSpace(NuevoPassword))
                usuario.Password = NuevoPassword;

            repository.Modificar(usuario);
            TempData["Mensaje"] = "Usuario actualizado correctamente";

            return RedirectToAction("Index");
        }

        // -------------------------
        // DETAILS
        // - Admin puede ver cualquiera
        // - Empleado sólo su propio detalle
        // -------------------------
        [HttpGet]
        public IActionResult Details(int id)
        {
            var currentId = GetUserId();
            var rol = GetUserRole();

            if (rol != "Administrador" && currentId != id)
                return Forbid();

            var u = repository.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // -------------------------
        // DELETE (GET confirm + POST) - SOLO ADMIN
        // -------------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return Forbid();

            var u = repository.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Forbid();

            repository.BajaLogica(id);
            TempData["Mensaje"] = "Usuario inhabilitado correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
    internal static class NullableExtensions
    {
        public static bool HasValue<T>(this T? value) where T : struct
            => value.HasValue;
    }
}
