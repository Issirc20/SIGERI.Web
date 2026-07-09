using FluentValidation;

namespace SIGERI.Application.Usuarios.Queries;

public sealed class ValidarCredencialesQueryValidator : AbstractValidator<ValidarCredencialesQuery>
{
    public ValidarCredencialesQueryValidator()
    {
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}
