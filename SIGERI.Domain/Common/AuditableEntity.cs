namespace SIGERI.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
	public string CreadoPor { get; set; }

	public string? ActualizadoPor { get; set; }

	protected AuditableEntity()
		: this(Guid.NewGuid(), DateTime.UtcNow, null, true, string.Empty, null)
	{
	}

	protected AuditableEntity(Guid id, DateTime fechaCreacion, DateTime? fechaActualizacion, bool activo, string creadoPor, string? actualizadoPor)
		: base(id, fechaCreacion, fechaActualizacion, activo)
	{
		CreadoPor = creadoPor;
		ActualizadoPor = actualizadoPor;
	}
}
