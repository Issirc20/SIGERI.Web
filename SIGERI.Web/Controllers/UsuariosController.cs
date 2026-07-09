using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Application.Usuarios.Commands;
using SIGERI.Application.Usuarios.Queries;
using SIGERI.Domain.Enums;
using System.Security.Claims;

namespace SIGERI.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery(), ct);
        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string nombre,
        string apellido,
        string correo,
        string password,
        RolUsuario rol,
        CancellationToken ct)
    {
        var creadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new RegistrarUsuarioCommand(nombre, apellido, correo, password, rol, creadoPor), ct);
            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new ObtenerUsuarioPorIdQuery(id), ct);
        if (usuario is null) return NotFound();
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        string nombre,
        string apellido,
        string correo,
        RolUsuario rol,
        bool estado,
        CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new ActualizarUsuarioCommand(id, nombre, apellido, correo, rol, estado, actualizadoPor), ct);
            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var usuario = await _mediator.Send(new ObtenerUsuarioPorIdQuery(id), ct);
            return View(usuario);
        }
    }

    [HttpGet]
    public async Task<IActionResult> CambiarPassword(Guid id, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new ObtenerUsuarioPorIdQuery(id), ct);
        if (usuario is null) return NotFound();
        ViewBag.UsuarioId    = id;
        ViewBag.UsuarioNombre = $"{usuario.Nombre} {usuario.Apellido}";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(
        Guid usuarioId,
        string nuevoPassword,
        CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        try
        {
            await _mediator.Send(new CambiarPasswordCommand(usuarioId, nuevoPassword, actualizadoPor), ct);
            TempData["Success"] = "Contraseña actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.UsuarioId = usuarioId;
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var actualizadoPor = User.FindFirstValue(ClaimTypes.Name) ?? "sistema";
        await _mediator.Send(new EliminarUsuarioCommand(id, actualizadoPor), ct);
        TempData["Success"] = "Usuario desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
