using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.PerfilesCriticos.Commands;
using SIGERI.Application.PerfilesCriticos.Queries;
using SIGERI.Domain.Enums;

namespace SIGERI.Web.Controllers;

[Authorize]
public class PerfilesCriticosController : Controller
{
    private readonly IMediator _mediator;

    public PerfilesCriticosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var perfiles = await _mediator.Send(new ObtenerPerfilesActivoCriticoQuery(), cancellationToken);
        return View(perfiles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid activoId, NivelImpactoOctave impactoReputacion, NivelImpactoOctave impactoFinanciero, NivelImpactoOctave impactoProductividad, NivelImpactoOctave impactoLegal, NivelImpactoOctave impactoSeguridadSsoma, string contenedoresTecnicos, string contenedoresFisicos, string contenedoresHumanos, string creadoPor, CancellationToken cancellationToken)
    {
        var command = new RegistrarPerfilActivoCriticoCommand(activoId, impactoReputacion, impactoFinanciero, impactoProductividad, impactoLegal, impactoSeguridadSsoma, contenedoresTecnicos, contenedoresFisicos, contenedoresHumanos, creadoPor);

        if (!ModelState.IsValid)
        {
            return View(command);
        }

        await _mediator.Send(command, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
