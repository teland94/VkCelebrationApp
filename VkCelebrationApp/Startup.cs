using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VkCelebrationApp.Autofac;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using System.Collections.Generic;
using VkCelebrationApp.ViewModels;
using VkCelebrationApp.BLL.MappingProfiles;
using VkCelebrationApp.Extensions;

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

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSwaggerDocumentation();

            services.AddAppAuth(Configuration);

            ApplicationContainer = services.AddAutofac(Configuration);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAppAuth();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwaggerDocumentation();

            loggerFactory.AddFile("logs/logger-info.txt");
            loggerFactory.AddFile("logs/logger.txt", LogLevel.Error);

            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ServiceProfile>();

                cfg.CreateMap(typeof(VkCollectionDto<>), typeof(VkCollectionViewModel<>))
                    .ConvertUsing(typeof(VkCollectionDtoToVkCollectionViewModelConverter<,>));

                cfg.CreateMap<VkUserDto, VkUserViewModel>();

                cfg.CreateMap<UserCongratulationDto, UserCongratulationViewModel>();

                cfg.CreateMap<SearchParamsViewModel, SearchParamsDto>()
                    .BeforeMap((s, d) => d.CanWritePrivateMessage = true);

                cfg.CreateMap<SearchUserParamsViewModel, SearchParamsDto>();
            });
        }

        #region Nested Classes

        private class VkCollectionDtoToVkCollectionViewModelConverter<TDto, TVm> : ITypeConverter<VkCollectionDto<TDto>, VkCollectionViewModel<TVm>>
        {
            public VkCollectionViewModel<TVm> Convert(VkCollectionDto<TDto> source, VkCollectionViewModel<TVm> destination, ResolutionContext context)
            {
                var usersVms = context.Mapper.Map<IEnumerable<TDto>, IEnumerable<TVm>>(source);

                return new VkCollectionViewModel<TVm>(source.TotalCount, usersVms);
            }
        }

        #endregion
    }
}
