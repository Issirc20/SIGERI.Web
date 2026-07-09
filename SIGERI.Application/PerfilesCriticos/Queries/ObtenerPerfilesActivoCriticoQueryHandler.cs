using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.PerfilesCriticos.Queries;

public sealed class ObtenerPerfilesActivoCriticoQueryHandler : IRequestHandler<ObtenerPerfilesActivoCriticoQuery, IReadOnlyCollection<PerfilActivoCriticoDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerPerfilesActivoCriticoQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PerfilActivoCriticoDto>> Handle(ObtenerPerfilesActivoCriticoQuery request, CancellationToken cancellationToken)
    {
        return await _context.PerfilesActivosCriticos
            .AsNoTracking()
            .Select(x => new PerfilActivoCriticoDto(
                x.Id,
                x.ActivoId,
                x.ActivoRelacionado.Nombre,
                x.ImpactoReputacion,
                x.ImpactoFinanciero,
                x.ImpactoProductividad,
                x.ImpactoLegal,
                x.ImpactoSeguridadSsoma,
                x.ContenedoresTecnicos,
                x.ContenedoresFisicos,
                x.ContenedoresHumanos))
            .ToListAsync(cancellationToken);
    }
}
