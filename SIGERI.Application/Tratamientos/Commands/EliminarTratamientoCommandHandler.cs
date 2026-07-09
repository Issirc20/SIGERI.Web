using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Tratamientos.Commands;

internal sealed class EliminarTratamientoCommandHandler : IRequestHandler<EliminarTratamientoCommand>
{
    private readonly ISigeriDbContext _context;

    public EliminarTratamientoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarTratamientoCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.PlanesTratamiento
            .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El plan de tratamiento no existe.");

        plan.Activo = false;
        plan.ActualizadoPor = request.ActualizadoPor;
        plan.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
