using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace _Net.Controllers;

public class ContratosController : Controller
{
    private readonly ContratosRepository repository;
    private readonly IConfiguration config;

    public ContratosController(IConfiguration config)
    {
        this.repository = new ContratosRepository(config);
        this.config = config;
    }

    public ActionResult Index(int? dias, bool? vigente)
{
    var lista = repository.ObtenerTodosOPorFiltros(dias, vigente);

    ViewBag.DiasOpciones = new SelectList(new[]
     {
        new { Value = "30", Text = "30 días" },
        new { Value = "60", Text = "60 días" },
        new { Value = "90", Text = "90 días" }
    }, "Value", "Text", dias?.ToString());

    ViewBag.DiasSeleccionado = dias;
    ViewBag.VigenteSeleccionado = vigente;

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
            repository.Alta(contrato);
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
        TempData["Mensaje"] = "Contrato eliminado correctamente";
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
