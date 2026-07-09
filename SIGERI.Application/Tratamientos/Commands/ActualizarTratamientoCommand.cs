using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Tratamientos.Commands;

public sealed record ActualizarTratamientoCommand(
    Guid Id,
    string Estrategia,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaTermino,
    EstadoPlan Estado,
    decimal CostoSalvaguarda,
    decimal PorcentajeMitigacion,
    decimal AleBase,
    string ActualizadoPor) : IRequest;
