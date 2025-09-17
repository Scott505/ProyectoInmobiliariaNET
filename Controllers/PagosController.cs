using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace _Net.Controllers;

public class PagosController : Controller
{
    private readonly PagosRepository repository;
    private readonly IConfiguration config;

    public PagosController(IConfiguration config)
    {
        this.repository = new PagosRepository(config);
        this.config = config;
    }

    public ActionResult Index()
    {
        var lista = repository.ObtenerTodos();
        return View(lista);
    }

    public ActionResult Create()
    {
        CargarDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Pago pago)
    {
        if (ModelState.IsValid)
        {
            repository.Alta(pago);
            TempData["Mensaje"] = "Pago registrado correctamente";
            return RedirectToAction(nameof(Index));
        }
        CargarDropdowns(pago);
        return View(pago);
    }

    public ActionResult Eliminar(int id)
    {
        var pago = repository.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View("Delete", pago);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmarEliminacion(int IdPago)
    {
        repository.Baja(IdPago); // Baja lógica: Anulado = true
        TempData["Mensaje"] = "Pago anulado correctamente";
        return RedirectToAction(nameof(Index));
    }

    public ActionResult Edit(int id)
    {
        var pago = repository.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        CargarDropdowns();
        return View(pago);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Pago pago)
    {
        if (ModelState.IsValid)
        {
            repository.Modificar(pago);
            TempData["Mensaje"] = "Pago actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }
        CargarDropdowns(pago);
        return View(pago);
    }

    public ActionResult Detalles(int id)
    {
        var pago = repository.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View(pago);
    }

    private void CargarDropdowns(Pago pago = null)
    {
        var contratos = new ContratosRepository(config).ObtenerTodos()
            .Where(c => c.Vigente) // solo contratos vigentes
            .Select(c => new
            {
                Id = c.IdContrato,
                Nombre = $"Contrato {c.IdContrato} - Inquilino {c.IdInquilino} - Inmueble {c.IdInmueble}"
            })
            .ToList();

        ViewBag.Contratos = new SelectList(contratos, "Id", "Nombre", pago?.IdContrato);
    }

}
