using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class Usuario : AuditableEntity
{
	public string Nombre { get; set; }

	public string Apellido { get; set; }

	public string Correo { get; set; }

	public string PasswordHash { get; set; }

	public RolUsuario Rol { get; set; }

	public bool Estado { get; set; }

	public ICollection<Evaluacion> Evaluaciones { get; set; }

	public Usuario()
	{
		Nombre = string.Empty;
		Apellido = string.Empty;
		Correo = string.Empty;
		PasswordHash = string.Empty;
		Evaluaciones = new List<Evaluacion>();
	}

	public Usuario(string nombre, string apellido, string correo, string passwordHash, RolUsuario rol, bool estado, ICollection<Evaluacion> evaluaciones)
	{
		Nombre = nombre;
		Apellido = apellido;
		Correo = correo;
		PasswordHash = passwordHash;
		Rol = rol;
		Estado = estado;
		Evaluaciones = evaluaciones;
	}
}
