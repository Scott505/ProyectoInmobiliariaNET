using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace _Net.Controllers;

[Authorize]
public class InmueblesController : Controller
{
    private readonly InmueblesRepository repository;
    private readonly IConfiguration config;
    public enum TipoInmueble { CASA, DEPARTAMENTO, LOCAL, DEPOSITO }
    public enum UsoInmueble { RESIENCIAL, COMERCIAL }

    public InmueblesController(IConfiguration config)
    {
        this.repository = new InmueblesRepository(config);
        this.config = config;
    }

    public ActionResult Index(bool? disponible, int? idPropietario)
    {
        var lista = repository.ObtenerTodosOPorFiltro(idPropietario, disponible);

        CargarPropietariosDropdown(idPropietario);
        ViewBag.DisponibleSeleccionado = disponible;

        return View(lista);
    }
    public ActionResult Create()
    {
        CargarDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Inmueble inmueble)
    {
        if (ModelState.IsValid)
        {
            repository.Alta(inmueble);
            TempData["Mensaje"] = "Inmueble creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        CargarDropdowns(inmueble);
        return View(inmueble);
    }

    [Authorize(Policy = "AdminOnly")]
    public ActionResult Eliminar(int id)
    {
        var inmueble = repository.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }
        return View("Delete", inmueble);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]

    public ActionResult ConfirmarEliminacion(int IdInmueble)
    {
        repository.Baja(IdInmueble); // lógica de baja lógica (Disponible = false)
        TempData["Mensaje"] = "Inmueble eliminado correctamente";
        return RedirectToAction(nameof(Index));
    }

    public ActionResult Edit(int id)
    {
        var inmueble = repository.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }
        CargarDropdowns();
        return View(inmueble);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Inmueble inmueble)
    {
        if (ModelState.IsValid)
        {
            repository.Modificar(inmueble);
            TempData["Mensaje"] = "Inmueble actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }
        CargarDropdowns(inmueble);
        return View(inmueble);
    }

    private void CargarDropdowns(Inmueble inmueble = null)
    {
        // Obtener propietarios
        var propietarios = new PropietariosRepository(config).ObtenerTodos()
            .Select(p => new { Id = p.IdPropietario, NombreCompleto = p.Nombre + " " + p.Apellido })
            .ToList();

        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");

        // Obtener enums
        ViewBag.Tipos = Enum.GetValues(typeof(TipoInmueble))
            .Cast<TipoInmueble>()
            .Select(t => new SelectListItem { Value = t.ToString(), Text = t.ToString() })
            .ToList();

        ViewBag.Usos = Enum.GetValues(typeof(UsoInmueble))
            .Cast<UsoInmueble>()
            .Select(u => new SelectListItem { Value = u.ToString(), Text = u.ToString() })
            .ToList();
    }

    private void CargarPropietariosDropdown(int? propietarioSeleccionado = null)
    {
        var propietarios = new PropietariosRepository(config).ObtenerTodos()
            .Select(p => new
            {
                Id = p.IdPropietario,
                NombreCompleto = p.Nombre + " " + p.Apellido
            })
            .ToList();

        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto", propietarioSeleccionado);
    }

}
