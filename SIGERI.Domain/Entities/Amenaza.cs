using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class Amenaza : AuditableEntity
{
	public string Codigo { get; set; }

	public string Nombre { get; set; }

	public string Descripcion { get; set; }

	public Guid CategoriaAmenazaId { get; set; }

	public CategoriaAmenaza Categoria { get; set; }

	public ICollection<Riesgo> Riesgos { get; set; }

	public Amenaza()
	{
		Codigo = string.Empty;
		Nombre = string.Empty;
		Descripcion = string.Empty;
		Categoria = null!;
		Riesgos = new List<Riesgo>();
	}

	public Amenaza(string codigo, string nombre, string descripcion, Guid categoriaAmenazaId, CategoriaAmenaza categoria, ICollection<Riesgo> riesgos)
	{
		Codigo = codigo;
		Nombre = nombre;
		Descripcion = descripcion;
		CategoriaAmenazaId = categoriaAmenazaId;
		Categoria = categoria;
		Riesgos = riesgos;
	}
}
