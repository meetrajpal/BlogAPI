using BlogAPI.API.Filters;
using Microsoft.OpenApi.Models;

namespace BlogAPI.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerExtention(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Blog API",
                Version = "v1",
                Description = "Blog API with JWT Authentication"
            });

            const string schemeId = "bearer";

            options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token only",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = schemeId,
                BearerFormat = "JWT"
            });

            // Use filter instead of global requirement
            options.OperationFilter<AuthOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseSwaggerWithVersioning(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API V1");
        });

        return app;
    }
}