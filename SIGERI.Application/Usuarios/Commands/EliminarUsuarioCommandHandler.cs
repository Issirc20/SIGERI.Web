using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Usuarios.Commands;

internal sealed class EliminarUsuarioCommandHandler : IRequestHandler<EliminarUsuarioCommand>
{
    private readonly ISigeriDbContext _context;

    public EliminarUsuarioCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El usuario no existe.");

        usuario.Activo = false;
        usuario.ActualizadoPor = request.ActualizadoPor;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
