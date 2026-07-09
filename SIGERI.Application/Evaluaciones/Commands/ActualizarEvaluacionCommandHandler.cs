using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Evaluaciones.Commands;

internal sealed class ActualizarEvaluacionCommandHandler : IRequestHandler<ActualizarEvaluacionCommand>
{
    private readonly ISigeriDbContext _context;

    public ActualizarEvaluacionCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await _context.Evaluaciones
            .SingleOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new DomainException("La evaluación no existe.");

        _ = await _context.Activos.AnyAsync(a => a.Id == request.ActivoId, cancellationToken)
            ? true
            : throw new DomainException("El activo especificado no existe.");

        _ = await _context.Usuarios.AnyAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ? true
            : throw new DomainException("El usuario especificado no existe.");

        evaluacion.Fecha = request.Fecha;
        evaluacion.Observaciones = request.Observaciones;
        evaluacion.UsuarioId = request.UsuarioId;
        evaluacion.ActivoId = request.ActivoId;
        evaluacion.ActualizadoPor = request.ActualizadoPor;
        evaluacion.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
