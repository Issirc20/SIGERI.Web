using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.ControlesMadurez.Commands;

public sealed record RegistrarControlMadurezCommand(
    string Codigo,
    string Nombre,
    FuncionNist Funcion,
    int NivelActual,
    int NivelObjetivo,
    string Descripcion,
    string CreadoPor) : IRequest<Guid>;
