using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class Activo : AuditableEntity
{
    public string Codigo { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public TipoActivo Tipo { get; set; }

    public string Propietario { get; set; }

    public string Ubicacion { get; set; }

    public Criticidad Criticidad { get; set; }

    public bool Estado { get; set; }

    public Guid OrganizacionId { get; set; }

    public Organizacion Organizacion { get; set; }

    public ICollection<Evaluacion> Evaluaciones { get; set; }

    public ICollection<DependenciaActivo> DependenciasOrigen { get; set; }

    public ICollection<DependenciaActivo> DependenciasDestino { get; set; }

    public ICollection<PerfilActivoCritico> PerfilesCriticos { get; set; }

    public Activo()
    {
        Codigo = string.Empty;
        Nombre = string.Empty;
        Descripcion = string.Empty;
        Propietario = string.Empty;
        Ubicacion = string.Empty;
        Organizacion = null!;
        Evaluaciones = new List<Evaluacion>();
        DependenciasOrigen = new List<DependenciaActivo>();
        DependenciasDestino = new List<DependenciaActivo>();
        PerfilesCriticos = new List<PerfilActivoCritico>();
    }

    public Activo(string codigo, string nombre, string descripcion, TipoActivo tipo, string propietario, string ubicacion, Criticidad criticidad, bool estado, Guid organizacionId, Organizacion organizacion, ICollection<Evaluacion> evaluaciones)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Tipo = tipo;
        Propietario = propietario;
        Ubicacion = ubicacion;
        Criticidad = criticidad;
        Estado = estado;
        OrganizacionId = organizacionId;
        Organizacion = organizacion;
        Evaluaciones = evaluaciones;
        DependenciasOrigen = new List<DependenciaActivo>();
        DependenciasDestino = new List<DependenciaActivo>();
        PerfilesCriticos = new List<PerfilActivoCritico>();
    }
}