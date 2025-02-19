using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.CommonLibrary.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddCustomSecurity(
            this IServiceCollection services, JwtOptions jwtOptions)
        {
            services.Configure<JwtOptions>(options =>
            {
                options.SecretKey = jwtOptions.SecretKey;
                options.Issuer = jwtOptions.Issuer;
                options.Audience = jwtOptions.Audience;
                options.ExpiryMinutes = jwtOptions.ExpiryMinutes;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(jwtOptions.SecretKey))
                };
            });

            services.AddScoped<ITokenValidator, TokenValidator>();

            return services;
        }
    }
}