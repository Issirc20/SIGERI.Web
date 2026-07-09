using FluentValidation;

namespace SIGERI.Application.Activos.Commands;

public sealed class EliminarActivoCommandValidator : AbstractValidator<EliminarActivoCommand>
{
    public EliminarActivoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador es obligatorio.");

        RuleFor(x => x.ActualizadoPor)
            .NotEmpty().WithMessage("El usuario que elimina es obligatorio.");
    }
}
