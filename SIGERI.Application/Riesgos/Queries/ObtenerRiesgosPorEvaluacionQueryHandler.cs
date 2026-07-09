using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Riesgos.Queries;

internal sealed class ObtenerRiesgosPorEvaluacionQueryHandler
    : IRequestHandler<ObtenerRiesgosPorEvaluacionQuery, IReadOnlyCollection<RiesgoDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerRiesgosPorEvaluacionQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<RiesgoDto>> Handle(
        ObtenerRiesgosPorEvaluacionQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Riesgos
            .AsNoTracking()
            .Where(r => r.EvaluacionId == request.EvaluacionId)
            .Include(r => r.Amenaza)
            .Include(r => r.Vulnerabilidad)
            .OrderByDescending(r => r.PuntajeRiesgo)
            .Select(r => new RiesgoDto(
                r.Id,
                r.Nombre,
                r.Descripcion,
                r.Probabilidad.Valor,
                r.Probabilidad.Descripcion,
                r.Impacto.Valor,
                r.Impacto.Descripcion,
                r.PuntajeRiesgo,
                r.NivelRiesgo,
                r.Estado,
                r.EvaluacionId,
                r.AmenazaId,
                r.Amenaza.Nombre,
                r.VulnerabilidadId,
                r.Vulnerabilidad.Nombre))
            .ToListAsync(cancellationToken);
    }
}
