using SIGERI.Domain.Common;

namespace SIGERI.Domain.Entities;

public class HistorialRiesgoResidual : BaseEntity
{
	public Guid RiesgoId { get; set; }

	public Riesgo Riesgo { get; set; }

	public int PuntajeOriginal { get; set; }

	public int PuntajeResidual { get; set; }

	public DateTime FechaCalculo { get; set; }

	public HistorialRiesgoResidual()
	{
		Riesgo = null!;
	}

	public HistorialRiesgoResidual(Guid riesgoId, Riesgo riesgo, int puntajeOriginal, int puntajeResidual, DateTime fechaCalculo)
	{
		RiesgoId = riesgoId;
		Riesgo = riesgo;
		PuntajeOriginal = puntajeOriginal;
		PuntajeResidual = puntajeResidual;
		FechaCalculo = fechaCalculo;
	}
}
