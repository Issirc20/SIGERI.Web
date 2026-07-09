using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Activos.Commands;
using SIGERI.Application.Activos.Queries;
using SIGERI.Application.DTOs;
using SIGERI.Domain.Enums;
using SIGERI.Web.ViewModels.Activos;
using System.Security.Claims;

namespace SIGERI.Web.Controllers;

[Authorize]
public class ActivosController : Controller
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;
    private static readonly Guid DefaultOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly IMediator _mediator;

    public ActivosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? criticidad, int page = 1, int pageSize = DefaultPageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ActivoDto> activos = await _mediator.Send(new ObtenerActivosQuery(), cancellationToken);

        string normalizedSearch = (search ?? string.Empty).Trim();
        string normalizedFilter = NormalizeCriticidadFilter(criticidad);
        int normalizedPageSize = NormalizePageSize(pageSize);

        IEnumerable<ActivoDto> query = activos;

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(a =>
                a.Codigo.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                a.Nombre.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                a.Tipo.ToString().Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                a.NombreOrganizacion.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
        }

        query = normalizedFilter switch
        {
            "critical" => query.Where(a => a.Criticidad == Criticidad.Critica),
            "high" => query.Where(a => a.Criticidad == Criticidad.Alta),
            "medium" => query.Where(a => a.Criticidad == Criticidad.Media),
            _ => query
        };

        List<ActivoDto> filtered = query
            .OrderBy(a => a.Codigo)
            .ThenBy(a => a.Nombre)
            .ToList();

        int totalItems = filtered.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)normalizedPageSize));
        int normalizedPage = Math.Clamp(page, 1, totalPages);

        List<ActivoDto> paged = filtered
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToList();

        var viewModel = new ActivosIndexViewModel
        {
            Activos = paged,
            SearchTerm = normalizedSearch,
            CriticidadFilter = normalizedFilter,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalItems = totalItems,
            CriticosCount = filtered.Count(a => a.Criticidad == Criticidad.Critica),
            AltosCount = filtered.Count(a => a.Criticidad == Criticidad.Alta),
            MediosCount = filtered.Count(a => a.Criticidad == Criticidad.Media)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string codigo,
        string nombre,
        string descripcion,
        TipoActivo tipoActivo,
        string propietario,
        string ubicacion,
        Criticidad criticidad,
        Guid organizacionId,
        string creadoPor,
        CancellationToken cancellationToken)
    {
        Guid resolvedOrganizationId = organizacionId == Guid.Empty ? DefaultOrganizationId : organizacionId;
        string resolvedCreatedBy = string.IsNullOrWhiteSpace(creadoPor) ? GetCurrentUserName() : creadoPor;

        var command = new RegistrarActivoCommand(
            codigo,
            nombre,
            descripcion,
            tipoActivo,
            propietario,
            ubicacion,
            criticidad,
            resolvedOrganizationId,
            resolvedCreatedBy);

        if (!ModelState.IsValid)
        {
            return View(command);
        }

        try
        {
            await _mediator.Send(command, cancellationToken);
            TempData["Success"] = "Activo registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(command);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var activo = await _mediator.Send(new ObtenerActivoPorIdQuery(id), cancellationToken);
        if (activo is null)
        {
            TempData["Error"] = "El activo solicitado no existe.";
            return RedirectToAction(nameof(Index));
        }

        var model = new ActualizarActivoCommand(
            activo.Id,
            activo.Codigo,
            activo.Nombre,
            activo.Descripcion,
            activo.Tipo,
            activo.Propietario,
            activo.Ubicacion,
            activo.Criticidad,
            activo.OrganizacionId,
            GetCurrentUserName());

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        string codigo,
        string nombre,
        string descripcion,
        TipoActivo tipoActivo,
        string propietario,
        string ubicacion,
        Criticidad criticidad,
        Guid organizacionId,
        CancellationToken cancellationToken)
    {
        Guid resolvedOrganizationId = organizacionId == Guid.Empty ? DefaultOrganizationId : organizacionId;

        var command = new ActualizarActivoCommand(
            id,
            codigo,
            nombre,
            descripcion,
            tipoActivo,
            propietario,
            ubicacion,
            criticidad,
            resolvedOrganizationId,
            GetCurrentUserName());

        if (!ModelState.IsValid)
        {
            return View(command);
        }

        try
        {
            await _mediator.Send(command, cancellationToken);
            TempData["Success"] = "Activo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(command);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new EliminarActivoCommand(id, GetCurrentUserName()), cancellationToken);
            TempData["Success"] = "Activo eliminado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static string NormalizeCriticidadFilter(string? criticidad)
    {
        string normalized = (criticidad ?? "all").Trim().ToLowerInvariant();
        return normalized is "critical" or "high" or "medium" ? normalized : "all";
    }

    private static int NormalizePageSize(int pageSize)
    {
        if (pageSize <= 0)
        {
            return DefaultPageSize;
        }

        return Math.Min(pageSize, MaxPageSize);
    }

    private string GetCurrentUserName()
    {
        return User.FindFirstValue(ClaimTypes.Name)
            ?? User.Identity?.Name
            ?? "sistema";
    }

    }

