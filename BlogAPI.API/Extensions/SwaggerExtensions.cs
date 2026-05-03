using Scalar.AspNetCore;

namespace BlogAPI.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication UseSwaggerWithVersioning(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }
}