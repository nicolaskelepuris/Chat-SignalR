using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration configuration)
        {
            var appName = configuration["Application:ApplicationName"];
            var assembly = configuration["Application:Assembly"];

            services.AddMediatR(AppDomain.CurrentDomain.Load(appName).GetType(assembly));

            return services;
        }

        public class ApplicationSettings
        {
            public string ApplicationName { get; set; }
            public List<string> Assemblies { get; set; }
        }
    }
}