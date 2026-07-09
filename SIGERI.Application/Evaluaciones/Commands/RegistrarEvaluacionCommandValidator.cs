using FluentValidation;

namespace SIGERI.Application.Evaluaciones.Commands;

public sealed class RegistrarEvaluacionCommandValidator : AbstractValidator<RegistrarEvaluacionCommand>
{
    public RegistrarEvaluacionCommandValidator()
    {
        RuleFor(x => x.Fecha).NotEmpty().WithMessage("La fecha es obligatoria.");
        RuleFor(x => x.Observaciones).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("El usuario responsable es obligatorio.");
        RuleFor(x => x.ActivoId).NotEmpty().WithMessage("El activo evaluado es obligatorio.");
        RuleFor(x => x.CreadoPor).NotEmpty();
    }
}
