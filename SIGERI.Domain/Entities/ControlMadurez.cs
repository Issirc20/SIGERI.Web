using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;

namespace SIGERI.Domain.Entities;

public class ControlMadurez : AuditableEntity
{
    public string Codigo { get; set; }

    public string Nombre { get; set; }

    public FuncionNist Funcion { get; set; }

    public int NivelActual { get; set; }

    public int NivelObjetivo { get; set; }

    public string Descripcion { get; set; }

    public ControlMadurez()
    {
        Codigo = string.Empty;
        Nombre = string.Empty;
        Descripcion = string.Empty;
    }

    public ControlMadurez(string codigo, string nombre, FuncionNist funcion, int nivelActual, int nivelObjetivo, string descripcion)
    {
        Codigo = codigo;
        Nombre = nombre;
        Funcion = funcion;
        NivelActual = nivelActual;
        NivelObjetivo = nivelObjetivo;
        Descripcion = descripcion;
    }
}
