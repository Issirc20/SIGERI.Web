using SIGERI.Application.DTOs;
using SIGERI.Domain.Enums;

namespace SIGERI.Web.ViewModels.Activos;

public sealed class ActivosIndexViewModel
{
    public required IReadOnlyCollection<ActivoDto> Activos { get; init; }

    public string SearchTerm { get; init; } = string.Empty;

    public string CriticidadFilter { get; init; } = "all";

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public int TotalItems { get; init; }

    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public int TotalActivos => TotalItems;

    public int CriticosCount { get; init; }

    public int AltosCount { get; init; }

    public int MediosCount { get; init; }

    public bool HasFiltersApplied => !string.IsNullOrWhiteSpace(SearchTerm) || !string.Equals(CriticidadFilter, "all", StringComparison.OrdinalIgnoreCase);

    public bool IsEmpty => TotalItems == 0;

    public string BuildRoute(int page)
    {
        var targetPage = page < 1 ? 1 : page;
        return $"?search={Uri.EscapeDataString(SearchTerm ?? string.Empty)}&criticidad={Uri.EscapeDataString(CriticidadFilter ?? "all")}&page={targetPage}&pageSize={PageSize}";
    }

    public bool MatchesCriticidad(Criticidad criticidad) =>
        CriticidadFilter switch
        {
            "critical" => criticidad == Criticidad.Critica,
            "high" => criticidad == Criticidad.Alta,
            "medium" => criticidad == Criticidad.Media,
            _ => true
        };
}
