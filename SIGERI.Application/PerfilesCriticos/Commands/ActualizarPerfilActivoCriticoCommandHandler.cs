using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;

namespace SIGERI.Application.PerfilesCriticos.Commands;

public sealed class ActualizarPerfilActivoCriticoCommandHandler : IRequestHandler<ActualizarPerfilActivoCriticoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public ActualizarPerfilActivoCriticoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ActualizarPerfilActivoCriticoCommand request, CancellationToken cancellationToken)
    {
        var perfil = await _context.PerfilesActivosCriticos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new DomainException("El perfil crítico no existe.");

        perfil.ActivoId = request.ActivoId;
        perfil.ImpactoReputacion = request.ImpactoReputacion;
        perfil.ImpactoFinanciero = request.ImpactoFinanciero;
        perfil.ImpactoProductividad = request.ImpactoProductividad;
        perfil.ImpactoLegal = request.ImpactoLegal;
        perfil.ImpactoSeguridadSsoma = request.ImpactoSeguridadSsoma;
        perfil.ContenedoresTecnicos = request.ContenedoresTecnicos.Trim();
        perfil.ContenedoresFisicos = request.ContenedoresFisicos.Trim();
        perfil.ContenedoresHumanos = request.ContenedoresHumanos.Trim();
        perfil.ActualizadoPor = request.ActualizadoPor.Trim();
        perfil.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return perfil.Id;
    }
}
