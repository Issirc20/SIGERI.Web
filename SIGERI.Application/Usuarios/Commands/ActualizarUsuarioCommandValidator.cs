using FluentValidation;

namespace SIGERI.Application.Usuarios.Commands;

public sealed class ActualizarUsuarioCommandValidator : AbstractValidator<ActualizarUsuarioCommand>
{
    public ActualizarUsuarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.ActualizadoPor).NotEmpty();
    }
}
