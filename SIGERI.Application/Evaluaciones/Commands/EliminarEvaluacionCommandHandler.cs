using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Evaluaciones.Commands;

internal sealed class EliminarEvaluacionCommandHandler : IRequestHandler<EliminarEvaluacionCommand>
{
    private readonly ISigeriDbContext _context;

    public EliminarEvaluacionCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await _context.Evaluaciones
            .SingleOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new DomainException("La evaluación no existe.");

        evaluacion.Activo = false;
        evaluacion.ActualizadoPor = request.ActualizadoPor;
        evaluacion.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
