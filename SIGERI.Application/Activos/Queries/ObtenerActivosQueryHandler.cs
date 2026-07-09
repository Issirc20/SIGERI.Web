using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Activos.Queries;

public sealed class ObtenerActivosQueryHandler : IRequestHandler<ObtenerActivosQuery, IReadOnlyCollection<ActivoDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerActivosQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ActivoDto>> Handle(ObtenerActivosQuery request, CancellationToken cancellationToken)
    {
        var activos = await _context.Activos
            .AsNoTracking()
            .Where(activo => activo.Estado)
            .Select(activo => new ActivoDto(
                activo.Id,
                activo.Codigo,
                activo.Nombre,
                activo.Descripcion,
                activo.Tipo,
                activo.Propietario,
                activo.Ubicacion,
                activo.Criticidad,
                activo.OrganizacionId,
                activo.Organizacion.Nombre,
                activo.Estado))
            .ToListAsync(cancellationToken);

        return activos;
    }
}
