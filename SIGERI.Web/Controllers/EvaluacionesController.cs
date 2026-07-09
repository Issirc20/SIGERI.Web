using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Activos.Queries;
using SIGERI.Application.Evaluaciones.Commands;
using SIGERI.Application.Evaluaciones.Queries;
using SIGERI.Application.Usuarios.Queries;
using System.Security.Claims;

namespace SIGERI.Web.Controllers;

[Authorize]
public class EvaluacionesController : Controller
{
    private readonly IMediator _mediator;

    public EvaluacionesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var evaluaciones = await _mediator.Send(new ObtenerEvaluacionesQuery(), ct);
        return View(evaluaciones);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var evaluacion = await _mediator.Send(new ObtenerEvaluacionPorIdQuery(id), ct);
        if (evaluacion is null) return NotFound();
        return View(evaluacion);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.Activos  = await _mediator.Send(new ObtenerActivosQuery(), ct);
        ViewBag.Usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        DateTime fecha,
        string observaciones,
        Guid usuarioId,
        Guid activoId,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Activos  = await _mediator.Send(new ObtenerActivosQuery(), ct);
            ViewBag.Usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
            return View();
        }

        var creadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new RegistrarEvaluacionCommand(fecha, observaciones, usuarioId, activoId, creadoPor), ct);
            TempData["Success"] = "Evaluación registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Activos  = await _mediator.Send(new ObtenerActivosQuery(), ct);
            ViewBag.Usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var evaluacion = await _mediator.Send(new ObtenerEvaluacionPorIdQuery(id), ct);
        if (evaluacion is null) return NotFound();
        ViewBag.Activos  = await _mediator.Send(new ObtenerActivosQuery(), ct);
        ViewBag.Usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
        return View(evaluacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        DateTime fecha,
        string observaciones,
        Guid usuarioId,
        Guid activoId,
        CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new ActualizarEvaluacionCommand(id, fecha, observaciones, usuarioId, activoId, actualizadoPor), ct);
            TempData["Success"] = "Evaluación actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Activos  = await _mediator.Send(new ObtenerActivosQuery(), ct);
            ViewBag.Usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
            var dto = await _mediator.Send(new ObtenerEvaluacionPorIdQuery(id), ct);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";
        await _mediator.Send(new EliminarEvaluacionCommand(id, actualizadoPor), ct);
        TempData["Success"] = "Evaluación eliminada.";
        return RedirectToAction(nameof(Index));
    }
}
