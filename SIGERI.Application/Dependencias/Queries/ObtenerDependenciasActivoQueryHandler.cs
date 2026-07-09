using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Dependencias.Queries;

public sealed class ObtenerDependenciasActivoQueryHandler : IRequestHandler<ObtenerDependenciasActivoQuery, IReadOnlyCollection<DependenciaActivoDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerDependenciasActivoQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<DependenciaActivoDto>> Handle(ObtenerDependenciasActivoQuery request, CancellationToken cancellationToken)
    {
        return await _context.DependenciasActivos
            .AsNoTracking()
            .Select(x => new DependenciaActivoDto(
                x.Id,
                x.ActivoOrigenId,
                x.ActivoOrigen.Nombre,
                x.ActivoDestinoId,
                x.ActivoDestino.Nombre,
                x.TipoDependencia,
                x.Descripcion,
                x.CriticidadOperativa))
            .ToListAsync(cancellationToken);
    }
}
