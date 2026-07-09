using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Evaluaciones.Queries;

internal sealed class ObtenerEvaluacionesQueryHandler
    : IRequestHandler<ObtenerEvaluacionesQuery, IReadOnlyCollection<EvaluacionDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerEvaluacionesQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<EvaluacionDto>> Handle(
        ObtenerEvaluacionesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Evaluaciones
            .AsNoTracking()
            .Include(e => e.Usuario)
            .Include(e => e.ActivoRelacionado)
            .OrderByDescending(e => e.Fecha)
            .Select(e => new EvaluacionDto(
                e.Id,
                e.Fecha,
                e.Observaciones,
                e.UsuarioId,
                $"{e.Usuario.Nombre} {e.Usuario.Apellido}",
                e.ActivoId,
                e.ActivoRelacionado.Nombre,
                e.Riesgos.Count))
            .ToListAsync(cancellationToken);
    }
}
