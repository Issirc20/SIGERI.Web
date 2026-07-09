using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Usuarios.Security;

namespace SIGERI.Application.Usuarios.Queries;

internal sealed class ValidarCredencialesQueryHandler
    : IRequestHandler<ValidarCredencialesQuery, UsuarioDto?>
{
    private readonly ISigeriDbContext _context;

    public ValidarCredencialesQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto?> Handle(ValidarCredencialesQuery request, CancellationToken cancellationToken)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Correo == correo && u.Estado)
            .SingleOrDefaultAsync(cancellationToken);

        if (usuario is null || !PasswordHashService.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            return null;
        }

        return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Apellido, usuario.Correo, usuario.Rol, usuario.Estado);
    }
}
