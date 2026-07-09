using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Dependencias.Commands;

public sealed class ActualizarDependenciaActivoCommandHandler : IRequestHandler<ActualizarDependenciaActivoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public ActualizarDependenciaActivoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ActualizarDependenciaActivoCommand request, CancellationToken cancellationToken)
    {
        var dependencia = await _context.DependenciasActivos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new DomainException("La dependencia no existe.");

        dependencia.ActivoOrigenId = request.ActivoOrigenId;
        dependencia.ActivoDestinoId = request.ActivoDestinoId;
        dependencia.TipoDependencia = request.TipoDependencia;
        dependencia.Descripcion = request.Descripcion.Trim();
        dependencia.CriticidadOperativa = request.CriticidadOperativa;
        dependencia.ActualizadoPor = request.ActualizadoPor.Trim();
        dependencia.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return dependencia.Id;
    }
}
