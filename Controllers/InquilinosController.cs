using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Authorization;
namespace _Net.Controllers;

[Authorize]
public class InquilinosController : Controller
{
    private readonly IRepositoryInquilinos repository;
    private readonly IConfiguration config;

    public InquilinosController(IRepositoryInquilinos repo,IConfiguration config)
    {
        this.repository = repo;
        this.config = config;
    }

    public ActionResult Index()
    {
        var lista = repository.ObtenerTodos();
        return View(lista);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Inquilino inquilino)
    {
        if (ModelState.IsValid)
        {
            repository.Alta(inquilino);
            return RedirectToAction(nameof(Index));
        }
        return View(inquilino);
    }

    [Authorize(Policy = "AdminOnly")]

    public ActionResult Eliminar(int id)
    {
        var Inquilino = repository.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound();
        }
        return View("Delete", Inquilino);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]

    public ActionResult ConfirmarEliminacion(int IdInquilino)
    {
        repository.Baja(IdInquilino);
        TempData["Mensaje"] = "Eliminación realizada correctamente";

        return RedirectToAction(nameof(Index));
    }

    public ActionResult Edit(int id)
    {
        var Inquilino = repository.ObtenerPorId(id);
        if (Inquilino == null)
        {
            return NotFound();
        }
        return View(Inquilino);
    }

    // POST: Inquilinos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Inquilino inquilino)
    {
        if (ModelState.IsValid)
        {
            repository.Modificar(inquilino);
            TempData["Mensaje"] = "Inquilino actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }
        return View(inquilino);
    }



}
