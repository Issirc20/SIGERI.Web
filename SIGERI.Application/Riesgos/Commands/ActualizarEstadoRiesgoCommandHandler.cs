using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Riesgos.Commands;

internal sealed class ActualizarEstadoRiesgoCommandHandler : IRequestHandler<ActualizarEstadoRiesgoCommand>
{
    private readonly ISigeriDbContext _context;

    public ActualizarEstadoRiesgoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarEstadoRiesgoCommand request, CancellationToken cancellationToken)
    {
        var riesgo = await _context.Riesgos
            .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El riesgo no existe.");

        var puntajeAnterior = riesgo.PuntajeRiesgo;
        riesgo.Estado = request.NuevoEstado;
        riesgo.ActualizadoPor = request.ActualizadoPor;
        riesgo.FechaActualizacion = DateTime.UtcNow;

        // Registrar historial del cambio de estado con el puntaje actual
        var historial = new HistorialRiesgoResidual(
            riesgoId: riesgo.Id,
            riesgo: riesgo,
            puntajeOriginal: puntajeAnterior,
            puntajeResidual: riesgo.PuntajeRiesgo,
            fechaCalculo: DateTime.UtcNow)
        {
            Id = Guid.NewGuid(),
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.HistorialesRiesgoResidual.Add(historial);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
