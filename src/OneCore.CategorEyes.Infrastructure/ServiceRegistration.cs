﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneCore.CategorEyes.Business;
using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Infrastructure.Persistence;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using OneCore.CategorEyes.Infrastructure.Persistence.Repositories;
using OneCore.CategorEyes.Infrastructure.Services;

namespace OneCore.CategorEyes.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CategorEyesDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("CategorEyes")));
            services.AddBusiness();
            services.AddScoped<IOpenAIService, OpenAIService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            return services;
        }
    }
}
