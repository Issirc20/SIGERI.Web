namespace SIGERI.Domain.ValueObjects;

public record Impacto
{
	public int Valor { get; init; }

	public string Descripcion { get; init; }

	public Impacto()
	{
		Descripcion = string.Empty;
	}

	public Impacto(int valor, string descripcion)
	{
		Valor = valor;
		Descripcion = descripcion;
	}
}
