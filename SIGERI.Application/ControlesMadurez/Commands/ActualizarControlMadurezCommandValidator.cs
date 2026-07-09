using FluentValidation;

namespace SIGERI.Application.ControlesMadurez.Commands;

public sealed class ActualizarControlMadurezCommandValidator : AbstractValidator<ActualizarControlMadurezCommand>
{
    public ActualizarControlMadurezCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador es obligatorio.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.NivelActual)
            .InclusiveBetween(0, 4).WithMessage("El nivel actual debe estar entre 0 y 4.");

        RuleFor(x => x.NivelObjetivo)
            .InclusiveBetween(0, 4).WithMessage("El nivel objetivo debe estar entre 0 y 4.");

        RuleFor(x => x.ActualizadoPor)
            .NotEmpty().WithMessage("El campo ActualizadoPor es obligatorio.");
    }
}
