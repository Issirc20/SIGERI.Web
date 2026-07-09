using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class Evaluacion : AuditableEntity
{
	public DateTime Fecha { get; set; }

	public string Observaciones { get; set; }

	public Guid UsuarioId { get; set; }

	public Usuario Usuario { get; set; }

	public Guid ActivoId { get; set; }

	public Activo ActivoRelacionado { get; set; }

	public ICollection<Riesgo> Riesgos { get; set; }

	public Evaluacion()
	{
		Observaciones = string.Empty;
		Usuario = null!;
		ActivoRelacionado = null!;
		Riesgos = new List<Riesgo>();
	}

	public Evaluacion(DateTime fecha, string observaciones, Guid usuarioId, Usuario usuario, Guid activoId, Activo activoRelacionado, ICollection<Riesgo> riesgos)
	{
		Fecha = fecha;
		Observaciones = observaciones;
		UsuarioId = usuarioId;
		Usuario = usuario;
		ActivoId = activoId;
		ActivoRelacionado = activoRelacionado;
		Riesgos = riesgos;
	}
}
