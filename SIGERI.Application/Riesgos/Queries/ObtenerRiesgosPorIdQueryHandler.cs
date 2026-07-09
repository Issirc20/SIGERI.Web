using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Riesgos.Queries;

internal sealed class ObtenerRiesgoPorIdQueryHandler
    : IRequestHandler<ObtenerRiesgoPorIdQuery, RiesgoDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerRiesgoPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<RiesgoDto?> Handle(
        ObtenerRiesgoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Riesgos
            .AsNoTracking()
            .Include(r => r.Amenaza)
            .Include(r => r.Vulnerabilidad)
            .Where(r => r.Id == request.Id)
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
            .SingleOrDefaultAsync(cancellationToken);
    }
}
