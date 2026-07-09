using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Usuarios.Queries;

internal sealed class ObtenerUsuarioPorIdQueryHandler
    : IRequestHandler<ObtenerUsuarioPorIdQuery, UsuarioDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerUsuarioPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto?> Handle(ObtenerUsuarioPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Apellido, u.Correo, u.Rol, u.Estado))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
