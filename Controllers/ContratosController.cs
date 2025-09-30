using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace _Net.Controllers;

[Authorize]
public class ContratosController : Controller
{
    private readonly ContratosRepository repository;
    private readonly IConfiguration config;

    public ContratosController(IConfiguration config)
    {
        this.repository = new ContratosRepository(config);
        this.config = config;
    }

public ActionResult Index(int? dias, bool? vigente, int? idInquilino, DateTime? fechaInicio, DateTime? fechaFin)
{
    var lista = repository.ObtenerTodosOPorFiltros(dias, vigente, idInquilino);

    ViewBag.DiasOpciones = new SelectList(new[] {
        new { Value = "30", Text = "30 días" },
        new { Value = "60", Text = "60 días" },
        new { Value = "90", Text = "90 días" }
    }, "Value", "Text", dias?.ToString());

    var inquilinos = new InquilinosRepository(config).ObtenerTodos()
        .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
        .ToList();
    ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto", idInquilino);

    ViewBag.DiasSeleccionado = dias;
    ViewBag.VigenteSeleccionado = vigente;
    ViewBag.InquilinoSeleccionado = idInquilino;

    // Inmuebles disponibles si se enviaron fechas
    if (fechaInicio.HasValue && fechaFin.HasValue)
    {
        var inmueblesRepo = new InmueblesRepository(config);
        ViewBag.InmueblesDisponibles = inmueblesRepo.ObtenerDisponiblesEntreFechas(fechaInicio.Value, fechaFin.Value);
        ViewBag.FechaInicio = fechaInicio.Value.ToShortDateString();
        ViewBag.FechaFin = fechaFin.Value.ToShortDateString();
    }

    return View(lista);
}

    public ActionResult Create()
    {
        CargarDropdowns();
        return View();
    }

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create(Contrato contrato)
{
    if (ModelState.IsValid)
    {
        // Validar superposición de fechas
        if (repository.ExisteSuperposicion(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin))
        {
            ModelState.AddModelError("", "El contrato se superpone con otro contrato vigente para este inmueble.");
            CargarDropdowns(contrato);
            return View(contrato);
        }

        repository.Alta(contrato);

        int userId = HttpContext.Session.GetInt32("UsuarioId")
                     ?? int.Parse(Request.Cookies["UsuarioId"]);

        var auditoriaRepo = new AuditoriasContratosRepository(config);
        auditoriaRepo.Insertar(new AuditoriaContrato
        {
            IdContrato = contrato.IdContrato,
            IdUsuarioCreador = userId,
            FechaCreacion = DateTime.Now
        });

        TempData["Mensaje"] = "Contrato creado correctamente";
        return RedirectToAction(nameof(Index));
    }
    CargarDropdowns(contrato);
    return View(contrato);
}

    public ActionResult Eliminar(int id)
    {
        var contrato = repository.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View("Delete", contrato);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmarEliminacion(int IdContrato)
{
    repository.Baja(IdContrato); // Baja lógica: Vigente = false

    // Registrar auditoría de finalización
    int userId = HttpContext.Session.GetInt32("UsuarioId")
                 ?? int.Parse(Request.Cookies["UsuarioId"]);

    var auditoriaRepo = new AuditoriasContratosRepository(config);
    auditoriaRepo.FinalizarContrato(IdContrato, userId, DateTime.Now);

    TempData["Mensaje"] = "Contrato finalizado correctamente";
    return RedirectToAction(nameof(Index));
}

    public ActionResult Edit(int id)
    {
        var contrato = repository.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }
        CargarDropdowns();
        return View(contrato);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit(Contrato contrato)
{
    if (ModelState.IsValid)
    {
        // Validar superposición de fechas excluyendo el contrato actual
        if (repository.ExisteSuperposicion(contrato.IdInmueble, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
        {
            ModelState.AddModelError("", "El contrato se superpone con otro contrato vigente para este inmueble.");
            CargarDropdowns(contrato);
            return View(contrato);
        }

        repository.Modificar(contrato);
        TempData["Mensaje"] = "Contrato actualizado correctamente";
        return RedirectToAction(nameof(Index));
    }
    CargarDropdowns(contrato);
    return View(contrato);
}

    public ActionResult Detalles(int id)
    {
        var contrato = repository.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }

    private void CargarDropdowns(Contrato contrato = null)
    {
        var inquilinos = new InquilinosRepository(config).ObtenerTodos()
            .Select(i => new { Id = i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
            .ToList();
        ViewBag.Inquilinos = new SelectList(inquilinos, "Id", "NombreCompleto", contrato?.IdInquilino);

        var inmuebles = new InmueblesRepository(config).ObtenerTodosOPorFiltro()
            .ToList();
        ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion", contrato?.IdInmueble);
    }


}