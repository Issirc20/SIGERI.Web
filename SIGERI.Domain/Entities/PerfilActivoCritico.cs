using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class PerfilActivoCritico : AuditableEntity
{
    public Guid ActivoId { get; set; }

    public Activo ActivoRelacionado { get; set; }

    public NivelImpactoOctave ImpactoReputacion { get; set; }

    public NivelImpactoOctave ImpactoFinanciero { get; set; }

    public NivelImpactoOctave ImpactoProductividad { get; set; }

    public NivelImpactoOctave ImpactoLegal { get; set; }

    public NivelImpactoOctave ImpactoSeguridadSsoma { get; set; }

    public string ContenedoresTecnicos { get; set; }

    public string ContenedoresFisicos { get; set; }

    public string ContenedoresHumanos { get; set; }

    public PerfilActivoCritico()
    {
        ContenedoresTecnicos = string.Empty;
        ContenedoresFisicos = string.Empty;
        ContenedoresHumanos = string.Empty;
        ActivoRelacionado = null!;
    }

    public PerfilActivoCritico(Guid activoId, Activo activoRelacionado, NivelImpactoOctave impactoReputacion, NivelImpactoOctave impactoFinanciero, NivelImpactoOctave impactoProductividad, NivelImpactoOctave impactoLegal, NivelImpactoOctave impactoSeguridadSsoma, string contenedoresTecnicos, string contenedoresFisicos, string contenedoresHumanos)
    {
        ActivoId = activoId;
        ActivoRelacionado = activoRelacionado;
        ImpactoReputacion = impactoReputacion;
        ImpactoFinanciero = impactoFinanciero;
        ImpactoProductividad = impactoProductividad;
        ImpactoLegal = impactoLegal;
        ImpactoSeguridadSsoma = impactoSeguridadSsoma;
        ContenedoresTecnicos = contenedoresTecnicos;
        ContenedoresFisicos = contenedoresFisicos;
        ContenedoresHumanos = contenedoresHumanos;
    }
}
