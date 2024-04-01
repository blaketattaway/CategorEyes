using Microsoft.Extensions.DependencyInjection;
using OneCore.CategorEyes.Business.Analysis;
using OneCore.CategorEyes.Business.Log;

namespace OneCore.CategorEyes.Business
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddScoped<IAnalysisBusiness, AnalysisBusiness>();
            services.AddScoped<ILogBusiness, LogBusiness>();
            return services;
        }
    }
}
