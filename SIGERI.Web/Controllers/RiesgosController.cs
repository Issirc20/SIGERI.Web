using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Riesgos.Commands;
using SIGERI.Application.Riesgos.Queries;
using SIGERI.Domain.Enums;
using System.Security.Claims;

namespace SIGERI.Web.Controllers;

[Authorize]
public class RiesgosController : Controller
{
    private readonly IMediator _mediator;

    public RiesgosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var riesgos = await _mediator.Send(new ObtenerRiesgosQuery(), ct);
        return View(riesgos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var riesgo = await _mediator.Send(new ObtenerRiesgoPorIdQuery(id), ct);
        if (riesgo is null) return NotFound();
        return View(riesgo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluar(
        [FromQuery] Guid activoId,
        Guid evaluacionId,
        Guid amenazaId,
        Guid vulnerabilidadId,
        int probabilidad,
        int impacto,
        string nombre,
        string descripcion,
        CancellationToken ct)
    {
        var creadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        var command = new EvaluarRiesgoCommand(
            activoId, evaluacionId, amenazaId, vulnerabilidadId,
            probabilidad, impacto, nombre, descripcion, creadoPor);

        try
        {
            await _mediator.Send(command, ct);
            TempData["Success"] = "Riesgo evaluado y registrado.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index", "Evaluaciones");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(
        Guid id,
        EstadoRiesgo nuevoEstado,
        CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new ActualizarEstadoRiesgoCommand(id, nuevoEstado, actualizadoPor), ct);
            TempData["Success"] = "Estado del riesgo actualizado.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
