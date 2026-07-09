using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Evaluaciones.Queries;

internal sealed class ObtenerEvaluacionPorIdQueryHandler
    : IRequestHandler<ObtenerEvaluacionPorIdQuery, EvaluacionDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerEvaluacionPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<EvaluacionDto?> Handle(
        ObtenerEvaluacionPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Evaluaciones
            .AsNoTracking()
            .Include(e => e.Usuario)
            .Include(e => e.ActivoRelacionado)
            .Where(e => e.Id == request.Id)
            .Select(e => new EvaluacionDto(
                e.Id,
                e.Fecha,
                e.Observaciones,
                e.UsuarioId,
                $"{e.Usuario.Nombre} {e.Usuario.Apellido}",
                e.ActivoId,
                e.ActivoRelacionado.Nombre,
                e.Riesgos.Count))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
