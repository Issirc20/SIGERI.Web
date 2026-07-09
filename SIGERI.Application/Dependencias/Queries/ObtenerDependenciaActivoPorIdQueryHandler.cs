using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Dependencias.Queries;

public sealed class ObtenerDependenciaActivoPorIdQueryHandler : IRequestHandler<ObtenerDependenciaActivoPorIdQuery, DependenciaActivoDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerDependenciaActivoPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<DependenciaActivoDto?> Handle(ObtenerDependenciaActivoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.DependenciasActivos
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new DependenciaActivoDto(
                x.Id,
                x.ActivoOrigenId,
                x.ActivoOrigen.Nombre,
                x.ActivoDestinoId,
                x.ActivoDestino.Nombre,
                x.TipoDependencia,
                x.Descripcion,
                x.CriticidadOperativa))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
