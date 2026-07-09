using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;
using SIGERI.Domain.ValueObjects;

namespace SIGERI.Domain.Entities;

public class Riesgo : AuditableEntity
{
	public string Nombre { get; set; }

	public string Descripcion { get; set; }

	public Probabilidad Probabilidad { get; set; } = null!;

	public Impacto Impacto { get; set; } = null!;

	public int PuntajeRiesgo { get; set; }

	public string NivelRiesgo { get; set; }

	public EstadoRiesgo Estado { get; set; }

	public Guid EvaluacionId { get; set; }

	public Evaluacion Evaluacion { get; set; }

	public Guid AmenazaId { get; set; }

	public Amenaza Amenaza { get; set; }

	public Guid VulnerabilidadId { get; set; }

	public Vulnerabilidad Vulnerabilidad { get; set; }

	public ICollection<PlanTratamiento> PlanesTratamiento { get; set; }

	public ICollection<HistorialRiesgoResidual> Historiales { get; set; }

	public Riesgo()
	{
		Nombre = string.Empty;
		Descripcion = string.Empty;
		NivelRiesgo = string.Empty;
		Evaluacion = null!;
		Amenaza = null!;
		Vulnerabilidad = null!;
		PlanesTratamiento = new List<PlanTratamiento>();
		Historiales = new List<HistorialRiesgoResidual>();
	}

	public Riesgo(string nombre, string descripcion, Probabilidad probabilidad, Impacto impacto, int puntajeRiesgo, string nivelRiesgo, EstadoRiesgo estado, Guid evaluacionId, Evaluacion evaluacion, Guid amenazaId, Amenaza amenaza, Guid vulnerabilidadId, Vulnerabilidad vulnerabilidad, ICollection<PlanTratamiento> planesTratamiento, ICollection<HistorialRiesgoResidual> historiales)
	{
		Nombre = nombre;
		Descripcion = descripcion;
		Probabilidad = probabilidad;
		Impacto = impacto;
		PuntajeRiesgo = puntajeRiesgo;
		NivelRiesgo = nivelRiesgo;
		Estado = estado;
		EvaluacionId = evaluacionId;
		Evaluacion = evaluacion;
		AmenazaId = amenazaId;
		Amenaza = amenaza;
		VulnerabilidadId = vulnerabilidadId;
		Vulnerabilidad = vulnerabilidad;
		PlanesTratamiento = planesTratamiento;
		Historiales = historiales;
	}
}
