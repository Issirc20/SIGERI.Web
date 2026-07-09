using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class Vulnerabilidad : AuditableEntity
{
	public string Codigo { get; set; }

	public string Nombre { get; set; }

	public string Descripcion { get; set; }

	public string SeveridadBase { get; set; }

	public ICollection<Riesgo> Riesgos { get; set; }

	public Vulnerabilidad()
	{
		Codigo = string.Empty;
		Nombre = string.Empty;
		Descripcion = string.Empty;
		SeveridadBase = string.Empty;
		Riesgos = new List<Riesgo>();
	}

	public Vulnerabilidad(string codigo, string nombre, string descripcion, string severidadBase, ICollection<Riesgo> riesgos)
	{
		Codigo = codigo;
		Nombre = nombre;
		Descripcion = descripcion;
		SeveridadBase = severidadBase;
		Riesgos = riesgos;
	}
}
