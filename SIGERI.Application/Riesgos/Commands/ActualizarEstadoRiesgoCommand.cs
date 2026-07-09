using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Riesgos.Commands;

public sealed record ActualizarEstadoRiesgoCommand(
    Guid Id,
    EstadoRiesgo NuevoEstado,
    string ActualizadoPor) : IRequest;
