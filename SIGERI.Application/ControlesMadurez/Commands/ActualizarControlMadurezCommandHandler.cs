using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.ControlesMadurez.Commands;

public sealed class ActualizarControlMadurezCommandHandler : IRequestHandler<ActualizarControlMadurezCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public ActualizarControlMadurezCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ActualizarControlMadurezCommand request, CancellationToken cancellationToken)
    {
        var control = await _context.ControlesMadurez.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El control de madurez no existe.");

        control.Codigo = request.Codigo.Trim();
        control.Nombre = request.Nombre.Trim();
        control.Funcion = request.Funcion;
        control.NivelActual = request.NivelActual;
        control.NivelObjetivo = request.NivelObjetivo;
        control.Descripcion = request.Descripcion.Trim();
        control.ActualizadoPor = request.ActualizadoPor.Trim();
        control.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return control.Id;
    }
}
