using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace BlogAPI.API.Extensions;

public static class OpenApiScalarExtensions
{
    public static IServiceCollection AddOpenApiService(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Blog API",
                    Version = "v1",
                    Description = "Blog API with JWT Authentication"
                };

                document.Components ??= new();
                document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter your JWT token here"
                    }
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseScalarWithVersioning(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Blog API";
            options.Theme = ScalarTheme.DeepSpace;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecuritySchemes = ["Bearer"]
            };
        });

        return app;
    }
}