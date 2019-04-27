using System;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using VkCelebrationApp.Autofac;
using VkCelebrationApp.Helpers;
using VkCelebrationApp.Configuration;
using Microsoft.Extensions.Options;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using System.Collections.Generic;
using VkCelebrationApp.ViewModels;
using VkCelebrationApp.BLL.MappingProfiles;

namespace VkCelebrationApp
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA22rtsjrdiffahse5illo3sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Vk Celebration API", Version = "v1" });
            });

            var jwtConfiguration = new JwtIssuerConfiguration();

            new ConfigureFromConfigurationOptions<JwtIssuerConfiguration>(Configuration.GetSection("JwtIssuer"))
                .Configure(jwtConfiguration);
            jwtConfiguration.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            services.AddSingleton(jwtConfiguration);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfiguration.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtConfiguration.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtConfiguration.Audience;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
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
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vk Celebration API V1");
            });

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

                cfg.CreateMap<VkUserDto, VkUserViewModel>()
                    .ForMember(x => x.Photo50, opt => opt.Ignore())
                    .ForMember(x => x.Photo100, opt => opt.Ignore());

                cfg.CreateMap<UserCongratulationDto, UserCongratulationViewModel>();
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
