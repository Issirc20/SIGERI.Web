namespace SIGERI.Domain.ValueObjects;

public record Probabilidad
{
	public int Valor { get; init; }

	public string Descripcion { get; init; }

	public Probabilidad()
	{
		Descripcion = string.Empty;
	}

	public Probabilidad(int valor, string descripcion)
	{
		Valor = valor;
		Descripcion = descripcion;
	}
}
