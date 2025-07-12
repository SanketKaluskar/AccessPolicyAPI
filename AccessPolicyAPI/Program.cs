using AccessPolicyAPI.Authorization;
using AccessPolicyAPI.Models;
using AccessPolicyAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AccessPolicyAPI;

public enum AuthenticationMode
{
    Production,
    Development,
    Test
}

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var modeValue = builder.Configuration["Authentication:Mode"];
        if (!Enum.TryParse(modeValue, ignoreCase: true, out AuthenticationMode authMode))
        {
            authMode = AuthenticationMode.Production;
        }

        if (authMode == AuthenticationMode.Development)
        {
            var secret = builder.Configuration["Authentication:Jwt:Secret"]; // From secrets.json, not appsettings.development.json
            var issuer = builder.Configuration["Authentication:Jwt:Issuer"];
            var audience = builder.Configuration["Authentication:Jwt:Audience"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = false, // For development, we don't care about token expiration
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                    };
                });
        }
        else
        {
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
        }

        builder.Services.Configure<ProjectPolicyOptions>(
            builder.Configuration.GetSection("AuthorizationPolicies:JuhuAdminPolicy"));
        builder.Services.Configure<ProjectPolicyOptions>(
            builder.Configuration.GetSection("AuthorizationPolicies:JuhuUserPolicy"));

        builder.Services.AddScoped<IAccessPolicyService, ConfigDrivenAccessPolicyService>();
        builder.Services.AddScoped<IAuthorizationHandler, ProjectAndRoleHandler>();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("JuhuAdminPolicy", policy => policy.Requirements.Add(new ProjectAndRoleRequirement()));
            options.AddPolicy("JuhuUserPolicy", policy => policy.Requirements.Add(new ProjectAndRoleRequirement()));
        });

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "AccessPolicyAPI", Version = "v1" });
        });

        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

