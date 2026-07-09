using System;
using System.Collections.Generic;

namespace SIGERI.Web.ViewModels.Analytics
{
    public class HeatmapPointDto
    {
        public int Impact { get; set; }
        public int Probability { get; set; }
        public string Id { get; set; } = string.Empty;
        public int Size { get; set; }
    }

    public class RecentRiskDto
    {
        public string RiskName { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }

    public class RecentActivityDto
    {
        public string Description { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty; // "Asset", "Evaluation", "Treatment", "Risk"
    }

    public class KpiCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Variant { get; set; } = "primary"; // "primary", "danger", "warning", "success", "info"
        public string Trend { get; set; } = string.Empty;
    }

    public class AnalyticsViewModel
    {
        public HeatmapPointDto[] HeatPoints { get; set; } = new HeatmapPointDto[0];
        public string[] RadarLabels { get; set; } = new string[0];
        public double[] RadarValues { get; set; } = new double[0];
        public string[] TrendLabels { get; set; } = new string[0];
        public double[] TrendValues { get; set; } = new double[0];
        public string[] DepsLabels { get; set; } = new string[0];
        public int[] DepsValues { get; set; } = new int[0];
        public string[] CategoryLabels { get; set; } = new string[0];
        public int[] CategoryValues { get; set; } = new int[0];
        public string[] LevelLabels { get; set; } = new string[0];
        public int[] LevelValues { get; set; } = new int[0];
        public string[] RosiLabels { get; set; } = new string[0];
        public int[] RosiValues { get; set; } = new int[0];

        // Summary metrics
        public int TotalAssets { get; set; }
        public int IdentifiedRisks { get; set; }
        public int CriticalRisks { get; set; }
        public int TreatmentsImplemented { get; set; }
        public int PendingRisks { get; set; }
        public int MaturityPercent { get; set; }
        public int ComplianceIso { get; set; }
        public int ComplianceNist { get; set; }
        public int ComplianceCis { get; set; }

        // New summary metrics
        public int ActiveTreatments { get; set; }
        public int OpenEvaluations { get; set; }

        // KPI financieros del dashboard
        public decimal AleExpuesto { get; set; }
        public decimal PlanInvestment { get; set; }
        public decimal AleResidual { get; set; }
        public decimal WeightedRosiPercent { get; set; }

        // New DTO collections for Dashboard
        public RecentRiskDto[] RecentRisks { get; set; } = new RecentRiskDto[0];
        public RecentActivityDto[] RecentActivities { get; set; } = new RecentActivityDto[0];
    }
}
