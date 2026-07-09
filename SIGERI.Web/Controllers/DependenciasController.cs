using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Dependencias.Commands;
using SIGERI.Application.Dependencias.Queries;
using SIGERI.Domain.Enums;

namespace SIGERI.Web.Controllers;

[Authorize]
public class DependenciasController : Controller
{
    private readonly IMediator _mediator;

    public DependenciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dependencias = await _mediator.Send(new ObtenerDependenciasActivoQuery(), cancellationToken);
        return View(dependencias);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid activoOrigenId, Guid activoDestinoId, TipoDependenciaActivo tipoDependencia, string descripcion, int criticidadOperativa, string creadoPor, CancellationToken cancellationToken)
    {
        var command = new RegistrarDependenciaActivoCommand(activoOrigenId, activoDestinoId, tipoDependencia, descripcion, criticidadOperativa, creadoPor);

        if (!ModelState.IsValid)
        {
            return View(command);
        }

        await _mediator.Send(command, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
