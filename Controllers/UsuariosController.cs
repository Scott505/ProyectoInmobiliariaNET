using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Http;

namespace _Net.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository repository;

        public UsuariosController(IConfiguration config)
        {
            repository = new UsuariosRepository(config);
        }

        // GET: /Usuarios/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Usuarios/Login
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

            // Guarda en sesión el rol que viene de la BD (sin elección por parte del usuario)
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre ?? string.Empty);

            TempData["Mensaje"] = $"Bienvenido {usuario.Nombre ?? usuario.Email}";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Usuarios/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // Solo Administradores pueden ver listado completo
        public IActionResult Index()
        {
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");
            if (rolSesion != "Administrador")
                return Forbid();

            var usuarios = repository.ObtenerTodos();
            return View(usuarios);
        }

        // Solo Administradores
        [HttpGet]
        public IActionResult Crear()
        {
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");
            if (rolSesion != "Administrador")
                return Forbid();

            return View(new Usuario());
        }

        // Solo Administradores
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Usuario usuario, string Password)
        {
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");
            if (rolSesion != "Administrador")
                return Forbid();

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

        // Admin puede editar cualquiera, Empleado solo su propio perfil
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var usuarioSesionId = HttpContext.Session.GetInt32("UsuarioId");
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");

            if (rolSesion != "Administrador" && usuarioSesionId != id)
                return Forbid();

            var u = repository.ObtenerPorId(id);
            if (u == null) return NotFound();

            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Usuario usuario, string NuevoPassword)
        {
            var usuarioSesionId = HttpContext.Session.GetInt32("UsuarioId");
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");

            if (rolSesion != "Administrador" && usuarioSesionId != usuario.IdUsuario)
                return Forbid();

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            // Si se envió nuevo password, actualizarlo; si no, conservar el existente
            if (!string.IsNullOrWhiteSpace(NuevoPassword))
            {
                usuario.Password = NuevoPassword;
            }
            else
            {
                // Obtener current para conservar password si no se cambió
                var actual = repository.ObtenerPorId(usuario.IdUsuario);
                if (actual != null)
                    usuario.Password = actual.Password;
            }

            repository.Modificar(usuario);
            TempData["Mensaje"] = "Usuario actualizado correctamente";

            // Si es admin, volver al listado; si es empleado, quedarse en su perfil
            if (rolSesion == "Administrador")
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Editar), new { id = usuario.IdUsuario });
        }

        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var usuarioSesionId = HttpContext.Session.GetInt32("UsuarioId");
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");

            if (rolSesion != "Administrador" && usuarioSesionId != id)
                return Forbid();

            var u = repository.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // Solo Administradores pueden dar de baja (baja lógica)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Baja(int id)
        {
            var rolSesion = HttpContext.Session.GetString("UsuarioRol");
            if (rolSesion != "Administrador")
                return Forbid();

            repository.BajaLogica(id);
            TempData["Mensaje"] = "Usuario inhabilitado correctamente";
            return RedirectToAction(nameof(Index));
        }

    }
}
