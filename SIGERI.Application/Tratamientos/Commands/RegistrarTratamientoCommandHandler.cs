using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Tratamientos.Commands;

internal sealed class RegistrarTratamientoCommandHandler : IRequestHandler<RegistrarTratamientoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarTratamientoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarTratamientoCommand request, CancellationToken cancellationToken)
    {
        var riesgo = await _context.Riesgos
            .SingleOrDefaultAsync(r => r.Id == request.RiesgoId, cancellationToken)
            ?? throw new DomainException("El riesgo especificado no existe.");

        var plan = new PlanTratamiento(
            riesgo.Id,
            riesgo,
            request.Estrategia,
            request.Descripcion,
            request.FechaInicio,
            request.FechaTermino,
            request.Estado,
            new List<Control>())
        {
            Id = Guid.NewGuid(),
            CreadoPor = request.CreadoPor,
            FechaCreacion = DateTime.UtcNow,
            Activo = true,
            CostoSalvaguarda = request.CostoSalvaguarda,
            PorcentajeMitigacion = request.PorcentajeMitigacion,
            AleBase = request.AleBase
        };

        _context.PlanesTratamiento.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);
        return plan.Id;
    }
}
