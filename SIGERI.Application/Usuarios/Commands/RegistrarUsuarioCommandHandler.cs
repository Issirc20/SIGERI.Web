using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Usuarios.Security;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Usuarios.Commands;

internal sealed class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarUsuarioCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();

        var existe = await _context.Usuarios
            .AnyAsync(u => u.Correo == correo, cancellationToken);

        if (existe)
            throw new DomainException($"Ya existe un usuario con el correo '{correo}'.");

        var hash = PasswordHashService.HashPassword(request.Password);

        var usuario = new Usuario(
            request.Nombre.Trim(),
            request.Apellido.Trim(),
            correo,
            hash,
            request.Rol,
            true,
            new List<Domain.Entities.Evaluacion>())
        {
            Id = Guid.NewGuid(),
            CreadoPor = request.CreadoPor,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);
        return usuario.Id;
    }
}
