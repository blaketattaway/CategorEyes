using Microsoft.Extensions.DependencyInjection;
using OneCore.CategorEyes.Business.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddScoped<IAnalysisBusiness, AnalysisBusiness>();
            return services;
        }
    }
}
