using FluentValidation;

namespace SIGERI.Application.Dependencias.Commands;

public sealed class ActualizarDependenciaActivoCommandValidator : AbstractValidator<ActualizarDependenciaActivoCommand>
{
    public ActualizarDependenciaActivoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador es obligatorio.");

        RuleFor(x => x.ActivoOrigenId)
            .NotEmpty().WithMessage("El activo origen es obligatorio.");

        RuleFor(x => x.ActivoDestinoId)
            .NotEmpty().WithMessage("El activo destino es obligatorio.")
            .NotEqual(x => x.ActivoOrigenId).WithMessage("El activo destino debe ser diferente al activo origen.");

        RuleFor(x => x.CriticidadOperativa)
            .InclusiveBetween(1, 5).WithMessage("La criticidad operativa debe estar entre 1 y 5.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede superar 500 caracteres.");

        RuleFor(x => x.ActualizadoPor)
            .NotEmpty().WithMessage("El campo ActualizadoPor es obligatorio.");
    }
}
