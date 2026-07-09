using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class DependenciaActivo : AuditableEntity
{
    public Guid ActivoOrigenId { get; set; }

    public Activo ActivoOrigen { get; set; }

    public Guid ActivoDestinoId { get; set; }

    public Activo ActivoDestino { get; set; }

    public TipoDependenciaActivo TipoDependencia { get; set; }

    public string Descripcion { get; set; }

    public int CriticidadOperativa { get; set; }

    public DependenciaActivo()
    {
        Descripcion = string.Empty;
        ActivoOrigen = null!;
        ActivoDestino = null!;
    }

    public DependenciaActivo(Guid activoOrigenId, Activo activoOrigen, Guid activoDestinoId, Activo activoDestino, TipoDependenciaActivo tipoDependencia, string descripcion, int criticidadOperativa)
    {
        ActivoOrigenId = activoOrigenId;
        ActivoOrigen = activoOrigen;
        ActivoDestinoId = activoDestinoId;
        ActivoDestino = activoDestino;
        TipoDependencia = tipoDependencia;
        Descripcion = descripcion;
        CriticidadOperativa = criticidadOperativa;
    }
}
