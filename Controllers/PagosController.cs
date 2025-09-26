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

    public ActionResult Index(int? idContrato)
    {
        var lista = repository.ObtenerTodosOPorFiltro(idContrato);

        var contratos = new ContratosRepository(config).ObtenerTodosOPorFiltros();
        ViewBag.Contratos = new SelectList(contratos, "IdContrato", "IdContrato", idContrato);
        ViewBag.ContratoSeleccionado = idContrato;

        return View(lista);
    }


   public ActionResult Create(int? idContrato = null)
{
    CargarDropdowns(null, idContrato);
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

    private void CargarDropdowns(Pago pago = null, int? idContratoFiltrado = null)
{
    var contratos = new ContratosRepository(config).ObtenerTodosOPorFiltros()
        .Select(c => new
        {
            Id = c.IdContrato,
            Nombre = $"Contrato {c.IdContrato}"
        })
        .ToList();

    // Insertamos opción vacía al inicio
    contratos.Insert(0, new { Id = 0, Nombre = "-- Seleccione --" });

    // Seleccionamos primero el contrato del pago si existe, sino usamos el filtrado
    int? seleccionado = pago?.IdContrato > 0 ? pago.IdContrato : idContratoFiltrado;

    ViewBag.Contratos = new SelectList(contratos, "Id", "Nombre", seleccionado);
}




}
