using MediatR;

namespace SIGERI.Application.Activos.Commands;

public sealed record EliminarActivoCommand(Guid Id, string ActualizadoPor) : IRequest<Guid>;
