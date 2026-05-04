using BlogAPI.API.Extensions;
using BlogAPI.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

// db
builder.Services.AddDatabase(builder.Configuration);

// identity
builder.Services.AddIdentityServices();

// jwt
builder.Services.AddJwtAuthentication(builder.Configuration);

// policies
builder.Services.AddAuthorizationPolicies();

// services
builder.Services.AddApplicationServices();

// redis
builder.Services.AddRedisCache(builder.Configuration);

// api versioning
builder.Services.AddApiVersioningServices();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExtention();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerWithVersioning();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();