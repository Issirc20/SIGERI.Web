using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.Activos.Commands;

public sealed class ActualizarActivoCommandHandler : IRequestHandler<ActualizarActivoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public ActualizarActivoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ActualizarActivoCommand request, CancellationToken cancellationToken)
    {
        var activo = await _context.Activos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El activo no existe.");

        activo.Codigo = request.Codigo.Trim();
        activo.Nombre = request.Nombre.Trim();
        activo.Descripcion = request.Descripcion.Trim();
        activo.Tipo = request.TipoActivo;
        activo.Propietario = request.Propietario.Trim();
        activo.Ubicacion = request.Ubicacion.Trim();
        activo.Criticidad = request.Criticidad;
        activo.OrganizacionId = request.OrganizacionId;
        activo.ActualizadoPor = request.ActualizadoPor.Trim();
        activo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return activo.Id;
    }
}
