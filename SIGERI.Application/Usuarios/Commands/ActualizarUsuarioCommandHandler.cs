using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Usuarios.Commands;

internal sealed class ActualizarUsuarioCommandHandler : IRequestHandler<ActualizarUsuarioCommand>
{
    private readonly ISigeriDbContext _context;

    public ActualizarUsuarioCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El usuario no existe.");

        var correo = request.Correo.Trim().ToLowerInvariant();

        var correoEnUso = await _context.Usuarios
            .AnyAsync(u => u.Correo == correo && u.Id != request.Id, cancellationToken);

        if (correoEnUso)
            throw new DomainException($"El correo '{correo}' ya está en uso por otro usuario.");

        usuario.Nombre = request.Nombre.Trim();
        usuario.Apellido = request.Apellido.Trim();
        usuario.Correo = correo;
        usuario.Rol = request.Rol;
        usuario.Estado = request.Estado;
        usuario.ActualizadoPor = request.ActualizadoPor;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
