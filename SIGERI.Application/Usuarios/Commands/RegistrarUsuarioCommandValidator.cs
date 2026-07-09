using FluentValidation;

namespace SIGERI.Application.Usuarios.Commands;

public sealed class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres.");
        RuleFor(x => x.CreadoPor).NotEmpty();
    }
}
