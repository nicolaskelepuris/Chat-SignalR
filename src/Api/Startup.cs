using Api.Extensions;
using Api.Middlewares;
using Application.Helpers;
using Application.Hubs;
using Domain.Interfaces;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Identity;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddDbContext<AppIdentityDbContext>(x => x.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddScoped<ITokenService, TokenService>();

            services.AddIdentityServices(_configuration);

            services.AddSwaggerApi(_configuration);

            services.AddMediator(_configuration);
            
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddHttpContextAccessor();

            services.AddApplicationServices();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwaggerDocApi(_configuration);

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("hubs/chat");
            });
        }
    }
}
