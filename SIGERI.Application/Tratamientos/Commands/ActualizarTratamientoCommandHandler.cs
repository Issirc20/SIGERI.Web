using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Tratamientos.Commands;

internal sealed class ActualizarTratamientoCommandHandler : IRequestHandler<ActualizarTratamientoCommand>
{
    private readonly ISigeriDbContext _context;

    public ActualizarTratamientoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarTratamientoCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.PlanesTratamiento
            .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El plan de tratamiento no existe.");

        plan.Estrategia = request.Estrategia;
        plan.Descripcion = request.Descripcion;
        plan.FechaInicio = request.FechaInicio;
        plan.FechaTermino = request.FechaTermino;
        plan.Estado = request.Estado;
        plan.CostoSalvaguarda = request.CostoSalvaguarda;
        plan.PorcentajeMitigacion = request.PorcentajeMitigacion;
        plan.AleBase = request.AleBase;
        plan.ActualizadoPor = request.ActualizadoPor;
        plan.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
