using Microsoft.AspNetCore.Mvc;
using _Net.Models;
using Microsoft.AspNetCore.Authorization;

namespace _Net.Controllers;

[Authorize]
public class PropietariosController : Controller
{
    private readonly PropietariosRepository repository;
    private readonly IConfiguration config;

    public PropietariosController(IConfiguration config)
    {
        this.repository = new PropietariosRepository(config);
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
    public ActionResult Create(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            repository.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
    }

    [Authorize(Policy = "AdminOnly")]

    public ActionResult Eliminar(int id)
    {
        var propietario = repository.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }
        return View("Delete", propietario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]

    public ActionResult ConfirmarEliminacion(int IdPropietario)
    {
        repository.Baja(IdPropietario);
        TempData["Mensaje"] = "Eliminación realizada correctamente";

        return RedirectToAction(nameof(Index));
    }


    public ActionResult Edit(int id)
    {
        var propietario = repository.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound(); 
        }
        return View(propietario); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            repository.Modificar(propietario);
            TempData["Mensaje"] = "Propietario actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
    }

}
