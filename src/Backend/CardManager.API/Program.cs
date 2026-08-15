using CardManager.API.Filters;
using CardManager.API.Middleware;
using CardManager.API.Token;
using CardManager.Application;
using CardManager.Communication.Responses;
using CardManager.Domain.Repositories.User;
using CardManager.Domain.Security.Tokens;
using CardManager.Exceptions;
using CardManager.Infrastructure;
using CardManager.Infrastructure.Extensions;
using CardManager.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter only your access token. Swagger will add 'Bearer' automatically.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITokenProvider, HttpContextTokenProvider>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        var signingKey = builder.Configuration.GetValue<string>("Jwt:SigninKey")!;

        jwtOptions.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.Zero
        };

        jwtOptions.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail("Invalid token subject");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadRepository>();

                var userExist = await userRepository.ExistActiveUserWithId(long.Parse(userId));
                if (!userExist)
                    context.Fail("User not found or inactive");
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = context.AuthenticateFailure switch
                {
                    null => new ResponseErrorJson(MessagesException.ACCESS_TOKEN_REQUIRED),
                    SecurityTokenExpiredException => new ResponseErrorJson("Token Expired", accessTokenExpired: true),
                    _ => new ResponseErrorJson(MessagesException.ACCESS_DENIED),
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

MigrateDatabase();

await app.RunAsync();


void MigrateDatabase()
{
    if (builder.Configuration.IsUniTestEnviromente())
        return;

    var databaseConnection = builder.Configuration.ConnectionString();
    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    DatabaseMigration.Migrate(databaseConnection, serviceScope.ServiceProvider);
}

public partial class Program
{
    protected Program() { }
}