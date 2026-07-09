using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class Control : AuditableEntity
{
	public Guid PlanTratamientoId { get; set; }

	public PlanTratamiento PlanTratamiento { get; set; }

	public string Codigo { get; set; }

	public string Nombre { get; set; }

	public string Descripcion { get; set; }

	public bool MitigaProbabilidad { get; set; }

	public bool MitigaImpacto { get; set; }

	public Control()
	{
		Codigo = string.Empty;
		Nombre = string.Empty;
		Descripcion = string.Empty;
		PlanTratamiento = null!;
	}

	public Control(Guid planTratamientoId, PlanTratamiento planTratamiento, string codigo, string nombre, string descripcion, bool mitigaProbabilidad, bool mitigaImpacto)
	{
		PlanTratamientoId = planTratamientoId;
		PlanTratamiento = planTratamiento;
		Codigo = codigo;
		Nombre = nombre;
		Descripcion = descripcion;
		MitigaProbabilidad = mitigaProbabilidad;
		MitigaImpacto = mitigaImpacto;
	}
}
