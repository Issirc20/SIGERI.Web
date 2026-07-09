using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Domain.ValueObjects;

namespace SIGERI.Application.Riesgos.Commands;

public sealed class EvaluarRiesgoCommandHandler : IRequestHandler<EvaluarRiesgoCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public EvaluarRiesgoCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(EvaluarRiesgoCommand request, CancellationToken cancellationToken)
    {
        if (request.Probabilidad is < 1 or > 5)
        {
            throw new DomainException("La probabilidad debe estar entre 1 y 5.");
        }

        if (request.Impacto is < 1 or > 5)
        {
            throw new DomainException("El impacto debe estar entre 1 y 5.");
        }

        var activo = await _context.Activos
            .SingleOrDefaultAsync(x => x.Id == request.ActivoId, cancellationToken)
            ?? throw new DomainException("El activo especificado no existe.");

        var evaluacion = await _context.Evaluaciones
            .SingleOrDefaultAsync(x => x.Id == request.EvaluacionId, cancellationToken)
            ?? throw new DomainException("La evaluación especificada no existe.");

        if (evaluacion.ActivoId != activo.Id)
        {
            throw new DomainException("La evaluación no corresponde al activo especificado.");
        }

        var amenaza = await _context.Amenazas
            .SingleOrDefaultAsync(x => x.Id == request.AmenazaId, cancellationToken)
            ?? throw new DomainException("La amenaza especificada no existe.");

        var vulnerabilidad = await _context.Vulnerabilidades
            .SingleOrDefaultAsync(x => x.Id == request.VulnerabilidadId, cancellationToken)
            ?? throw new DomainException("La vulnerabilidad especificada no existe.");

        var puntajeRiesgo = request.Probabilidad * request.Impacto;
        var nivelRiesgo = ObtenerNivelRiesgo(puntajeRiesgo);

        var riesgo = new Riesgo
        {
            Nombre = request.NombreRiesgo.Trim(),
            Descripcion = request.DescripcionRiesgo.Trim(),
            Probabilidad = new Probabilidad(request.Probabilidad, ObtenerDescripcionProbabilidad(request.Probabilidad)),
            Impacto = new Impacto(request.Impacto, ObtenerDescripcionImpacto(request.Impacto)),
            PuntajeRiesgo = puntajeRiesgo,
            NivelRiesgo = nivelRiesgo,
            Estado = EstadoRiesgo.Evaluado,
            EvaluacionId = evaluacion.Id,
            Evaluacion = evaluacion,
            AmenazaId = amenaza.Id,
            Amenaza = amenaza,
            VulnerabilidadId = vulnerabilidad.Id,
            Vulnerabilidad = vulnerabilidad,
            CreadoPor = request.CreadoPor.Trim()
        };

        _context.Riesgos.Add(riesgo);
        await _context.SaveChangesAsync(cancellationToken);

        return riesgo.Id;
    }

    private static string ObtenerNivelRiesgo(int puntajeRiesgo) => puntajeRiesgo switch
    {
        <= 4 => "Bajo",
        <= 9 => "Medio",
        <= 15 => "Alto",
        <= 25 => "Crítico",
        _ => throw new DomainException("El puntaje de riesgo debe estar entre 1 y 25.")
    };

    private static string ObtenerDescripcionProbabilidad(int valor) => valor switch
    {
        1 => "Muy baja",
        2 => "Baja",
        3 => "Media",
        4 => "Alta",
        5 => "Muy alta",
        _ => throw new DomainException("La probabilidad debe estar entre 1 y 5.")
    };

    private static string ObtenerDescripcionImpacto(int valor) => valor switch
    {
        1 => "Muy bajo",
        2 => "Bajo",
        3 => "Medio",
        4 => "Alto",
        5 => "Muy alto",
        _ => throw new DomainException("El impacto debe estar entre 1 y 5.")
    };
}
