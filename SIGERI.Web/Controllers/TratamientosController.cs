using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Riesgos.Queries;
using SIGERI.Application.Tratamientos.Commands;
using SIGERI.Application.Tratamientos.Queries;
using SIGERI.Domain.Enums;
using System.Security.Claims;

namespace SIGERI.Web.Controllers;

[Authorize]
public class TratamientosController : Controller
{
    private readonly IMediator _mediator;

    public TratamientosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var tratamientos = await _mediator.Send(new ObtenerTratamientosQuery(), ct);
        return View(tratamientos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var plan = await _mediator.Send(new ObtenerTratamientoPorIdQuery(id), ct);
        if (plan is null) return NotFound();
        return View(plan);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.Riesgos = await _mediator.Send(new ObtenerRiesgosQuery(), ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Guid riesgoId,
        string estrategia,
        string descripcion,
        DateTime fechaInicio,
        DateTime fechaTermino,
        EstadoPlan estado,
        decimal costoSalvaguarda,
        decimal porcentajeMitigacion,
        decimal aleBase,
        CancellationToken ct)
    {
        var creadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new RegistrarTratamientoCommand(
                riesgoId, estrategia, descripcion,
                fechaInicio, fechaTermino, estado,
                costoSalvaguarda, porcentajeMitigacion, aleBase, creadoPor), ct);
            TempData["Success"] = "Plan de tratamiento registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Riesgos = await _mediator.Send(new ObtenerRiesgosQuery(), ct);
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var plan = await _mediator.Send(new ObtenerTratamientoPorIdQuery(id), ct);
        if (plan is null) return NotFound();
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        string estrategia,
        string descripcion,
        DateTime fechaInicio,
        DateTime fechaTermino,
        EstadoPlan estado,
        decimal costoSalvaguarda,
        decimal porcentajeMitigacion,
        decimal aleBase,
        CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new ActualizarTratamientoCommand(
                id, estrategia, descripcion,
                fechaInicio, fechaTermino, estado,
                costoSalvaguarda, porcentajeMitigacion, aleBase, actualizadoPor), ct);
            TempData["Success"] = "Plan actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var plan = await _mediator.Send(new ObtenerTratamientoPorIdQuery(id), ct);
            return View(plan);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";
        await _mediator.Send(new EliminarTratamientoCommand(id, actualizadoPor), ct);
        TempData["Success"] = "Plan de tratamiento eliminado.";
        return RedirectToAction(nameof(Index));
    }
}
