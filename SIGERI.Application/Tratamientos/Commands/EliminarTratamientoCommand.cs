using MediatR;

namespace SIGERI.Application.Tratamientos.Commands;

public sealed record EliminarTratamientoCommand(Guid Id, string ActualizadoPor) : IRequest;
