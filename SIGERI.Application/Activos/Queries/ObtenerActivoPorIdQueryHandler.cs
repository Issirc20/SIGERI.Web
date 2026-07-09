using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Activos.Queries;

public sealed class ObtenerActivoPorIdQueryHandler : IRequestHandler<ObtenerActivoPorIdQuery, ActivoDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerActivoPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<ActivoDto?> Handle(ObtenerActivoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Activos
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
