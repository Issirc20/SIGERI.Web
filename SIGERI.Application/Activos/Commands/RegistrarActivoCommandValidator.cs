using FluentValidation;

namespace SIGERI.Application.Activos.Commands;

public sealed class RegistrarActivoCommandValidator : AbstractValidator<RegistrarActivoCommand>
{
    public RegistrarActivoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty();

        RuleFor(x => x.Nombre)
            .NotEmpty();

        RuleFor(x => x.OrganizacionId)
            .NotEmpty();
    }
}
