using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.ControlesMadurez.Commands;
using SIGERI.Application.ControlesMadurez.Queries;
using SIGERI.Domain.Enums;

namespace SIGERI.Web.Controllers;

[Authorize]
public class ControlesMadurezController : Controller
{
    private readonly IMediator _mediator;

    public ControlesMadurezController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var controles = await _mediator.Send(new ObtenerControlesMadurezQuery(), cancellationToken);
        return View(controles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string codigo, string nombre, FuncionNist funcion, int nivelActual, int nivelObjetivo, string descripcion, string creadoPor, CancellationToken cancellationToken)
    {
        var command = new RegistrarControlMadurezCommand(codigo, nombre, funcion, nivelActual, nivelObjetivo, descripcion, creadoPor);

        if (!ModelState.IsValid)
        {
            return View(command);
        }

        await _mediator.Send(command, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
