using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Authorization;
[Authorize]
public class AuditoriasController : Controller
{
    private readonly IConfiguration config;
    private readonly AuditoriasContratosRepository contratosRepo;
    private readonly AuditoriaPagosRepository pagosRepo;

    public AuditoriasController(IConfiguration config)
    {
        this.config = config;
        this.contratosRepo = new AuditoriasContratosRepository(config);
        this.pagosRepo = new AuditoriaPagosRepository(config);
    }

    // Vista inicial con menú de opciones
    public IActionResult Index()
    {
        return View();
    }

    // Auditoría de contratos
    public IActionResult Contratos()
    {
        var auditorias = contratosRepo.ObtenerTodos();
        return View(auditorias ?? new List<AuditoriaContrato>());
    }

    // Auditoría de pagos
    public IActionResult Pagos()
    {
        var auditorias = pagosRepo.ObtenerTodos();
        return View(auditorias ?? new List<AuditoriaPago>());
    }
}
