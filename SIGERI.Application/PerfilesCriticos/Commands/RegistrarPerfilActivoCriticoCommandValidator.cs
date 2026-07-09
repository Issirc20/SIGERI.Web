using FluentValidation;

namespace SIGERI.Application.PerfilesCriticos.Commands;

public sealed class RegistrarPerfilActivoCriticoCommandValidator : AbstractValidator<RegistrarPerfilActivoCriticoCommand>
{
    public RegistrarPerfilActivoCriticoCommandValidator()
    {
        RuleFor(x => x.ActivoId)
            .NotEmpty().WithMessage("El activo es obligatorio.");

        RuleFor(x => x.ContenedoresTecnicos)
            .NotEmpty().WithMessage("Los contenedores técnicos son obligatorios.")
            .MaximumLength(500).WithMessage("No puede superar 500 caracteres.");

        RuleFor(x => x.ContenedoresFisicos)
            .NotEmpty().WithMessage("Los contenedores físicos son obligatorios.")
            .MaximumLength(500).WithMessage("No puede superar 500 caracteres.");

        RuleFor(x => x.ContenedoresHumanos)
            .NotEmpty().WithMessage("Los contenedores humanos son obligatorios.")
            .MaximumLength(500).WithMessage("No puede superar 500 caracteres.");

        RuleFor(x => x.CreadoPor)
            .NotEmpty().WithMessage("El campo CreadoPor es obligatorio.");
    }
}
