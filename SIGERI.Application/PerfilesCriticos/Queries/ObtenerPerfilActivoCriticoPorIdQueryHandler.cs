using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.PerfilesCriticos.Queries;

public sealed class ObtenerPerfilActivoCriticoPorIdQueryHandler : IRequestHandler<ObtenerPerfilActivoCriticoPorIdQuery, PerfilActivoCriticoDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerPerfilActivoCriticoPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<PerfilActivoCriticoDto?> Handle(ObtenerPerfilActivoCriticoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.PerfilesActivosCriticos
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
