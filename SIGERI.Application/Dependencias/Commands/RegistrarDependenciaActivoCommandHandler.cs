using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Dependencias.Commands;

public sealed class RegistrarDependenciaActivoCommandHandler : IRequestHandler<RegistrarDependenciaActivoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarDependenciaActivoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarDependenciaActivoCommand request, CancellationToken cancellationToken)
    {
        var origen = await _context.Activos.SingleOrDefaultAsync(x => x.Id == request.ActivoOrigenId, cancellationToken)
            ?? throw new DomainException("El activo origen no existe.");

        var destino = await _context.Activos.SingleOrDefaultAsync(x => x.Id == request.ActivoDestinoId, cancellationToken)
            ?? throw new DomainException("El activo destino no existe.");

        if (request.CriticidadOperativa is < 1 or > 5)
        {
            throw new DomainException("La criticidad operativa debe estar entre 1 y 5.");
        }

        var dependency = new DependenciaActivo
        {
            ActivoOrigenId = origen.Id,
            ActivoOrigen = origen,
            ActivoDestinoId = destino.Id,
            ActivoDestino = destino,
            TipoDependencia = request.TipoDependencia,
            Descripcion = request.Descripcion.Trim(),
            CriticidadOperativa = request.CriticidadOperativa,
            CreadoPor = request.CreadoPor.Trim()
        };

        _context.DependenciasActivos.Add(dependency);
        await _context.SaveChangesAsync(cancellationToken);

        return dependency.Id;
    }
}
