using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Activos.Commands;

public sealed class EliminarActivoCommandHandler : IRequestHandler<EliminarActivoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public EliminarActivoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(EliminarActivoCommand request, CancellationToken cancellationToken)
    {
        var activo = await _context.Activos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El activo no existe.");

        activo.Estado = false;
        activo.ActualizadoPor = request.ActualizadoPor.Trim();
        activo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return activo.Id;
    }
}
