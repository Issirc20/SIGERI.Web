using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class PlanTratamiento : AuditableEntity
{
	public Guid RiesgoId { get; set; }

	public Riesgo Riesgo { get; set; }

	public string Estrategia { get; set; }

	public string Descripcion { get; set; }

	public DateTime FechaInicio { get; set; }

	public DateTime FechaTermino { get; set; }

	public EstadoPlan Estado { get; set; }

	/// <summary>Costo total de la salvaguarda/control a implementar (S/).</summary>
	public decimal CostoSalvaguarda { get; set; }

	/// <summary>Porcentaje de reducción del riesgo esperado (0–100).</summary>
	public decimal PorcentajeMitigacion { get; set; }

	/// <summary>ALE base antes de aplicar el tratamiento (S/).</summary>
	public decimal AleBase { get; set; }

	public ICollection<Control> Controles { get; set; }

	public PlanTratamiento()
	{
		Estrategia = string.Empty;
		Descripcion = string.Empty;
		Riesgo = null!;
		Controles = new List<Control>();
	}

	public PlanTratamiento(Guid riesgoId, Riesgo riesgo, string estrategia, string descripcion, DateTime fechaInicio, DateTime fechaTermino, EstadoPlan estado, ICollection<Control> controles)
	{
		RiesgoId = riesgoId;
		Riesgo = riesgo;
		Estrategia = estrategia;
		Descripcion = descripcion;
		FechaInicio = fechaInicio;
		FechaTermino = fechaTermino;
		Estado = estado;
		Controles = controles;
	}
}
