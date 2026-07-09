using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.PerfilesCriticos.Commands;

public sealed class RegistrarPerfilActivoCriticoCommandHandler : IRequestHandler<RegistrarPerfilActivoCriticoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarPerfilActivoCriticoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarPerfilActivoCriticoCommand request, CancellationToken cancellationToken)
    {
        var activo = await _context.Activos.SingleOrDefaultAsync(x => x.Id == request.ActivoId, cancellationToken)
            ?? throw new DomainException("El activo especificado no existe.");

        var perfil = new PerfilActivoCritico
        {
            ActivoId = activo.Id,
            ActivoRelacionado = activo,
            ImpactoReputacion = request.ImpactoReputacion,
            ImpactoFinanciero = request.ImpactoFinanciero,
            ImpactoProductividad = request.ImpactoProductividad,
            ImpactoLegal = request.ImpactoLegal,
            ImpactoSeguridadSsoma = request.ImpactoSeguridadSsoma,
            ContenedoresTecnicos = request.ContenedoresTecnicos.Trim(),
            ContenedoresFisicos = request.ContenedoresFisicos.Trim(),
            ContenedoresHumanos = request.ContenedoresHumanos.Trim(),
            CreadoPor = request.CreadoPor.Trim()
        };

        _context.PerfilesActivosCriticos.Add(perfil);
        await _context.SaveChangesAsync(cancellationToken);

        return perfil.Id;
    }
}
