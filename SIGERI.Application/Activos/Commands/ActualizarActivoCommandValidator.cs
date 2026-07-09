using FluentValidation;

namespace SIGERI.Application.Activos.Commands;

public sealed class ActualizarActivoCommandValidator : AbstractValidator<ActualizarActivoCommand>
{
    public ActualizarActivoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador es obligatorio.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.");

        RuleFor(x => x.OrganizacionId)
            .NotEmpty().WithMessage("La organización es obligatoria.");

        RuleFor(x => x.ActualizadoPor)
            .NotEmpty().WithMessage("El campo ActualizadoPor es obligatorio.");
    }
}
