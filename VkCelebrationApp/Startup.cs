using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using VkCelebrationApp.Autofac;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Vk Celebration API", Version = "v1" });
            });

            ApplicationContainer = services.AddAutofac(Configuration);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vk Celebration API V1");
            });

            loggerFactory.AddFile("logs/logger-info.txt");
            loggerFactory.AddFile("logs/logger.txt", LogLevel.Error);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            AuthVk();
            StartBot();
        }

        private void AuthVk()
        {
            using (var scope = ApplicationContainer.BeginLifetimeScope())
            {
                var vkApi = scope.Resolve<IVkCelebrationService>();
                vkApi.Auth();
            }
        }

        private void StartBot()
        {
            using (var scope = ApplicationContainer.BeginLifetimeScope())
            {
                var telegramBotService = scope.Resolve<IVkCelebrationTelegramBotService>();
                telegramBotService.InitClient();
            }
        }
    }
}
