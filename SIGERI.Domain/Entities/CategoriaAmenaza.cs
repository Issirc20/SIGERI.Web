using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class CategoriaAmenaza : BaseEntity
{
	public string Nombre { get; set; }

	public string Descripcion { get; set; }

	public ICollection<Amenaza> Amenazas { get; set; }

	public CategoriaAmenaza()
	{
		Nombre = string.Empty;
		Descripcion = string.Empty;
		Amenazas = new List<Amenaza>();
	}

	public CategoriaAmenaza(string nombre, string descripcion, ICollection<Amenaza> amenazas)
	{
		Nombre = nombre;
		Descripcion = descripcion;
		Amenazas = amenazas;
	}
}
