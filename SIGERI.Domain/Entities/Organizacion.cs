using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class Organizacion : AuditableEntity
{
	public string Nombre { get; set; }

	public string Siglas { get; set; }

	public ICollection<Activo> Activos { get; set; }

	public Organizacion()
	{
		Nombre = string.Empty;
		Siglas = string.Empty;
		Activos = new List<Activo>();
	}

	public Organizacion(string nombre, string siglas, ICollection<Activo> activos)
	{
		Nombre = nombre;
		Siglas = siglas;
		Activos = activos;
	}
}
