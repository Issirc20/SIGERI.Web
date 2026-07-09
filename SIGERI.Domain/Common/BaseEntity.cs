namespace SIGERI.Domain.Common;

public abstract class BaseEntity
{
	public Guid Id { get; set; }

	public DateTime FechaCreacion { get; set; }

	public DateTime? FechaActualizacion { get; set; }

	public bool Activo { get; set; }

	protected BaseEntity()
		: this(Guid.NewGuid(), DateTime.UtcNow, null, true)
	{
	}

	protected BaseEntity(Guid id, DateTime fechaCreacion, DateTime? fechaActualizacion, bool activo)
	{
		Id = id;
		FechaCreacion = fechaCreacion;
		FechaActualizacion = fechaActualizacion;
		Activo = activo;
	}
}
