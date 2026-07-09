using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record PlanTratamientoDto(
    Guid Id,
    Guid RiesgoId,
    string RiesgoNombre,
    string Estrategia,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaTermino,
    EstadoPlan Estado,
    decimal CostoSalvaguarda,
    decimal PorcentajeMitigacion,
    decimal AleBase,
    decimal AleResidual,
    decimal Rosi,
    int TotalControles);
