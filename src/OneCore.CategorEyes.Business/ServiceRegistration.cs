using Microsoft.Extensions.DependencyInjection;
using OneCore.CategorEyes.Business.Analysis;
using OneCore.CategorEyes.Business.Log;
using OneCore.CategorEyes.Business.Report;

namespace OneCore.CategorEyes.Business
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddScoped<IAnalysisBusiness, AnalysisBusiness>();
            services.AddScoped<ILogBusiness, LogBusiness>();
            services.AddScoped<IReportBusiness, ReportBusiness>();
            return services;
        }
    }
}
