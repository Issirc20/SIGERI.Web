using MediatR;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Activos.Commands;

public sealed class RegistrarActivoCommandHandler : IRequestHandler<RegistrarActivoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarActivoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarActivoCommand request, CancellationToken cancellationToken)
    {
        var activo = new Activo
        {
            Codigo = request.Codigo.Trim(),
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion.Trim(),
            Tipo = request.TipoActivo,
            Propietario = request.Propietario.Trim(),
            Ubicacion = request.Ubicacion.Trim(),
            Criticidad = request.Criticidad,
            Estado = true,
            OrganizacionId = request.OrganizacionId,
            CreadoPor = request.CreadoPor.Trim()
        };

        _context.Activos.Add(activo);
        await _context.SaveChangesAsync(cancellationToken);

        return activo.Id;
    }
}
