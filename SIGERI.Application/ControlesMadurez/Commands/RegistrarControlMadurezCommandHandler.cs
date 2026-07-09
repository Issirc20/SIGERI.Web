using MediatR;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.ControlesMadurez.Commands;

public sealed class RegistrarControlMadurezCommandHandler : IRequestHandler<RegistrarControlMadurezCommand, Guid>
{
    private readonly ISigeriDbContext _context;

    public RegistrarControlMadurezCommandHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegistrarControlMadurezCommand request, CancellationToken cancellationToken)
    {
        var control = new ControlMadurez
        {
            Codigo = request.Codigo.Trim(),
            Nombre = request.Nombre.Trim(),
            Funcion = request.Funcion,
            NivelActual = request.NivelActual,
            NivelObjetivo = request.NivelObjetivo,
            Descripcion = request.Descripcion.Trim(),
            CreadoPor = request.CreadoPor.Trim()
        };

        _context.ControlesMadurez.Add(control);
        await _context.SaveChangesAsync(cancellationToken);

        return control.Id;
    }
}
