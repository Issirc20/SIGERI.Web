using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Usuarios.Security;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Usuarios.Commands;

internal sealed class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand>
{
    private readonly ISigeriDbContext _context;

    public CambiarPasswordCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CambiarPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new DomainException("El usuario no existe.");

        usuario.PasswordHash = PasswordHashService.HashPassword(request.NuevoPassword);
        usuario.ActualizadoPor = request.ActualizadoPor;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
