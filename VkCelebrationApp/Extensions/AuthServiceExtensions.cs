using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using VkCelebrationApp.Configuration;

namespace VkCelebrationApp.Extensions
{
    public static class AuthServiceExtensions
    {
        private const string SecretKey = "iNivDmHLpUA22rtsjrdiffahse5illo3sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private static readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static IServiceCollection AddAppAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtIssuerConfiguration();

            new ConfigureFromConfigurationOptions<JwtIssuerConfiguration>(configuration.GetSection("JwtIssuer"))
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
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, 
                    Helpers.Constants.Strings.JwtClaims.ApiAccess));
            });

            return services;
        }

        public static IApplicationBuilder UseAppAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            return app;
        }
    }
}
