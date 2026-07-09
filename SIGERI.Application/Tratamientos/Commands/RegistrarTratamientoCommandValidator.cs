using FluentValidation;

namespace SIGERI.Application.Tratamientos.Commands;

public sealed class RegistrarTratamientoCommandValidator : AbstractValidator<RegistrarTratamientoCommand>
{
    public RegistrarTratamientoCommandValidator()
    {
        RuleFor(x => x.RiesgoId).NotEmpty().WithMessage("El riesgo asociado es obligatorio.");
        RuleFor(x => x.Estrategia).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.FechaInicio).NotEmpty();
        RuleFor(x => x.FechaTermino)
            .NotEmpty()
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha de término debe ser posterior a la fecha de inicio.");
        RuleFor(x => x.CostoSalvaguarda)
            .GreaterThanOrEqualTo(0).WithMessage("El costo de salvaguarda no puede ser negativo.");
        RuleFor(x => x.PorcentajeMitigacion)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje de mitigación debe estar entre 0 y 100.");
        RuleFor(x => x.AleBase)
            .GreaterThanOrEqualTo(0).WithMessage("El ALE base no puede ser negativo.");
        RuleFor(x => x.CreadoPor).NotEmpty();
    }
}
