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
            var applicationSettings = new ApplicationSettings();
            configuration.GetSection("Application").Bind(applicationSettings);

            if (applicationSettings == null)
                throw new ArgumentNullException(nameof(applicationSettings));

            foreach (var assembly in applicationSettings.Assemblies){
                services.AddMediatR(AppDomain.CurrentDomain.Load(applicationSettings.ApplicationName).GetType(assembly));
            }

            return services;
        }

        public class ApplicationSettings
        {
            public string ApplicationName { get; set; }
            public List<string> Assemblies { get; set; }
        }
    }
}