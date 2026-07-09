using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Usuarios.Queries;

internal sealed class ObtenerUsuariosQueryHandler
    : IRequestHandler<ObtenerUsuariosQuery, IReadOnlyCollection<UsuarioDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerUsuariosQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UsuarioDto>> Handle(ObtenerUsuariosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Apellido, u.Correo, u.Rol, u.Estado))
            .ToListAsync(cancellationToken);
    }
}
