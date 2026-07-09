using System;
using System.Threading.Tasks;
using SIGERI.Web.ViewModels.Analytics;

namespace SIGERI.Web.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetAnalyticsViewModelAsync(System.Nullable<Guid> userId);
    }
}
