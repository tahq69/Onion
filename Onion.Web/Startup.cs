using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Onion.Application;
using Onion.Application.Interfaces;
using Onion.Data;
using Onion.Identity;
using Onion.Infrastructure;
using Onion.Logging;
using Onion.Web.Middlewares;
using Onion.Web.Services;

namespace Onion.Web
{
    /// <summary>
    /// Web application start-up class.
    /// </summary>
    public partial class Startup
    {
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="env">The web host environment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the application services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();

            services.AddInfrastructure(Configuration);
            services.AddIdentityInfrastructure(Configuration);
            services.AddDataInfrastructure(Configuration);
            services.AddApplicationInfrastructure(Configuration);

            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddSingleton<IApiUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());

                return new ApiUriService(uri);
            });

            ConfigureSwagger(services);
            ConfigureApiVersioning(services);

            if (_env.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(
                        "AllowOrigin",
                        builder => builder
                            .WithOrigins("http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });
            }
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CorrelationIdLoggingMiddleware>();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowOrigin");
            }

            var assetPath = Path.Combine(_env.ContentRootPath, "Assets", "build");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(assetPath),
            });
            app.UseRouting();
            app.UseRequestLoggingMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("index.html");
                    return Task.CompletedTask;
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Onion"); });
        }
    }
}