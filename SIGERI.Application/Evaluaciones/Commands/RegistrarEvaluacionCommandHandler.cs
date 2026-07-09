using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Evaluaciones.Commands;

internal sealed class RegistrarEvaluacionCommandHandler : IRequestHandler<RegistrarEvaluacionCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarEvaluacionCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var activo = await _context.Activos
            .SingleOrDefaultAsync(a => a.Id == request.ActivoId, cancellationToken)
            ?? throw new DomainException("El activo especificado no existe.");

        var usuario = await _context.Usuarios
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new DomainException("El usuario especificado no existe.");

        var evaluacion = new Evaluacion(
            request.Fecha,
            request.Observaciones,
            usuario.Id,
            usuario,
            activo.Id,
            activo,
            new List<Riesgo>())
        {
            Id = Guid.NewGuid(),
            CreadoPor = request.CreadoPor,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.Evaluaciones.Add(evaluacion);
        await _context.SaveChangesAsync(cancellationToken);
        return evaluacion.Id;
    }
}
