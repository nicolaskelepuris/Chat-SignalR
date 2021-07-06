using System.Linq;
using Application.Responses;
using Application.Responses.Errors;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;

namespace Api.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errorMessage = actionContext.ModelState
                        .Where(m => m.Value.Errors.Any())
                        .SelectMany(m => m.Value.Errors.Select(e => e.ErrorMessage))
                        .Aggregate("", (current, value) => current + value + "; ");

                    errorMessage = errorMessage.Remove(errorMessage.Length - 2);

                    var errorResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Error = new ErrorResponse(statusCode: 400, message: errorMessage)
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}