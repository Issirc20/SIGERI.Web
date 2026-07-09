using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record ControlMadurezDto(
    Guid Id,
    string Codigo,
    string Nombre,
    FuncionNist Funcion,
    int NivelActual,
    int NivelObjetivo,
    string Descripcion);
