using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Enums;
using SIGERI.Web.ViewModels.Analytics;

namespace SIGERI.Web.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ISigeriDbContext _db;

        public AnalyticsService(ISigeriDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsViewModelAsync(System.Nullable<Guid> userId)
        {
            var model = new AnalyticsViewModel();

            var evaluacionesQuery = _db.Evaluaciones.AsNoTracking();
            if (userId.HasValue)
            {
                evaluacionesQuery = evaluacionesQuery.Where(e => e.UsuarioId == userId.Value);
            }

            var evaluacionIds = await evaluacionesQuery.Select(e => e.Id).ToListAsync();
            var activoIds = await evaluacionesQuery.Select(e => e.ActivoId).Distinct().ToListAsync();

            var riesgosQuery = _db.Riesgos
                .AsNoTracking()
                .Where(r => evaluacionIds.Contains(r.EvaluacionId));
            var riesgoIds = await riesgosQuery.Select(r => r.Id).ToListAsync();
            var planesQuery = _db.PlanesTratamiento
                .AsNoTracking()
                .Where(p => riesgoIds.Contains(p.RiesgoId));

            model.TotalAssets = activoIds.Count;
            model.IdentifiedRisks = await riesgosQuery.CountAsync();
            model.CriticalRisks = await riesgosQuery.CountAsync(r => r.Impacto.Valor >= 4 || r.Probabilidad.Valor >= 4);
            model.TreatmentsImplemented = await planesQuery.CountAsync(p => p.Estado == EstadoPlan.Completado);
            model.PendingRisks = await riesgosQuery.CountAsync(r => !r.PlanesTratamiento.Any(p => p.Estado == EstadoPlan.Completado));
            model.ActiveTreatments = await planesQuery.CountAsync(p => p.Estado != EstadoPlan.Completado);
            model.OpenEvaluations = await evaluacionesQuery.CountAsync();

            var controlesTotal = await _db.ControlesMadurez.AsNoTracking().CountAsync();
            var controlesImplementados = await _db.ControlesMadurez.AsNoTracking().CountAsync(c => c.NivelActual >= c.NivelObjetivo);
            model.MaturityPercent = controlesTotal > 0 ? (int)((controlesImplementados * 100m) / controlesTotal) : 0;
            model.ComplianceIso = model.MaturityPercent;
            model.ComplianceNist = model.MaturityPercent;
            model.ComplianceCis = model.MaturityPercent;

            model.AleExpuesto = (await planesQuery.Select(p => (decimal?)p.AleBase).SumAsync()) ?? 0m;
            model.PlanInvestment = (await planesQuery.Select(p => (decimal?)p.CostoSalvaguarda).SumAsync()) ?? 0m;
            model.AleResidual = (await planesQuery
                .Select(p => (decimal?)(p.AleBase * (1m - (p.PorcentajeMitigacion / 100m))))
                .SumAsync()) ?? 0m;

            if (model.PlanInvestment > 0m)
            {
                var beneficio = model.AleExpuesto - model.AleResidual;
                model.WeightedRosiPercent = Math.Round(((beneficio - model.PlanInvestment) / model.PlanInvestment) * 100m, 2);
            }

            model.HeatPoints = await riesgosQuery
                .Select(r => new HeatmapPointDto
                {
                    Impact = r.Impacto.Valor,
                    Probability = r.Probabilidad.Valor,
                    Id = r.Id.ToString(),
                    Size = 8
                })
                .ToArrayAsync();

            var radar = await _db.ControlesMadurez
                .AsNoTracking()
                .GroupBy(c => c.Funcion)
                .Select(g => new
                {
                    Label = g.Key.ToString(),
                    Value = g.Average(x => (double)x.NivelActual)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            model.RadarLabels = radar.Select(r => r.Label).ToArray();
            model.RadarValues = radar.Select(r => Math.Round(r.Value, 2)).ToArray();

            var plans = await planesQuery.OrderBy(p => p.FechaCreacion).ToListAsync();
            model.TrendLabels = plans.Select((p, i) => $"PT-{i + 1}").ToArray();
            model.TrendValues = plans.Select(p => (double)p.CostoSalvaguarda).ToArray();

            model.DepsLabels = await _db.DependenciasActivos
                .AsNoTracking()
                .Where(d => activoIds.Contains(d.ActivoOrigenId))
                .GroupBy(d => d.ActivoOrigen.Nombre)
                .Select(g => g.Key)
                .ToArrayAsync();

            model.DepsValues = await _db.DependenciasActivos
                .AsNoTracking()
                .Where(d => activoIds.Contains(d.ActivoOrigenId))
                .GroupBy(d => d.ActivoOrigen.Nombre)
                .Select(g => g.Count())
                .ToArrayAsync();

            model.CategoryLabels = await riesgosQuery
                .GroupBy(r => r.Amenaza.Categoria.Nombre)
                .Select(g => g.Key)
                .ToArrayAsync();

            model.CategoryValues = await riesgosQuery
                .GroupBy(r => r.Amenaza.Categoria.Nombre)
                .Select(g => g.Count())
                .ToArrayAsync();

            model.LevelLabels = new[] { "Crítico", "Alto", "Medio", "Bajo" };
            var levels = await riesgosQuery
                .GroupBy(r => r.NivelRiesgo)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToListAsync();

            model.LevelValues = new[]
            {
                levels.FirstOrDefault(l => l.Level == "Crítico")?.Count ?? 0,
                levels.FirstOrDefault(l => l.Level == "Alto")?.Count ?? 0,
                levels.FirstOrDefault(l => l.Level == "Medio")?.Count ?? 0,
                levels.FirstOrDefault(l => l.Level == "Bajo")?.Count ?? 0
            };

            var rosiList = await planesQuery
                .Select(p => new
                {
                    Initiative = p.Estrategia,
                    Rosi = p.CostoSalvaguarda > 0m
                        ? (((p.AleBase - (p.AleBase * (1m - (p.PorcentajeMitigacion / 100m)))) - p.CostoSalvaguarda) / p.CostoSalvaguarda) * 100m
                        : 0m
                })
                .ToListAsync();

            model.RosiLabels = rosiList.Select(r => r.Initiative ?? string.Empty).ToArray();
            model.RosiValues = rosiList.Select(r => (int)Math.Round(r.Rosi, 0)).ToArray();

            // Populate Recent Risks
            model.RecentRisks = await riesgosQuery
                .OrderByDescending(r => r.FechaCreacion)
                .Take(5)
                .Select(r => new RecentRiskDto
                {
                    RiskName = r.Nombre,
                    AssetName = r.Evaluacion.ActivoRelacionado.Nombre,
                    Level = r.NivelRiesgo,
                    Score = r.PuntajeRiesgo,
                    Date = r.FechaCreacion
                })
                .ToArrayAsync();

            // Populate Recent Activity Feed
            var recentAssets = await _db.Activos
                .AsNoTracking()
                .OrderByDescending(a => a.FechaCreacion)
                .Take(5)
                .Select(a => new RecentActivityDto
                {
                    Description = $"Activo registrado: {a.Nombre} ({a.Codigo})",
                    User = a.CreadoPor ?? "Sistema",
                    Date = a.FechaCreacion,
                    Type = "Asset"
                })
                .ToListAsync();

            var recentEvaluations = await evaluacionesQuery
                .OrderByDescending(e => e.FechaCreacion)
                .Take(5)
                .Select(e => new RecentActivityDto
                {
                    Description = e.Observaciones.Length > 45 
                        ? $"Evaluación creada: {e.Observaciones.Substring(0, 42)}..." 
                        : $"Evaluación creada: {e.Observaciones}",
                    User = e.CreadoPor ?? "Sistema",
                    Date = e.FechaCreacion,
                    Type = "Evaluation"
                })
                .ToListAsync();

            var recentTreatments = await planesQuery
                .OrderByDescending(p => p.FechaCreacion)
                .Take(5)
                .Select(p => new RecentActivityDto
                {
                    Description = $"Tratamiento iniciado: {p.Estrategia}",
                    User = p.CreadoPor ?? "Sistema",
                    Date = p.FechaCreacion,
                    Type = "Treatment"
                })
                .ToListAsync();

            model.RecentActivities = recentAssets
                .Concat(recentEvaluations)
                .Concat(recentTreatments)
                .OrderByDescending(a => a.Date)
                .Take(6)
                .ToArray();

            return model;
        }
    }
}
