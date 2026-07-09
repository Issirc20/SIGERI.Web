using MediatR;

namespace SIGERI.Application.Evaluaciones.Commands;

public sealed record EliminarEvaluacionCommand(Guid Id, string ActualizadoPor) : IRequest;
