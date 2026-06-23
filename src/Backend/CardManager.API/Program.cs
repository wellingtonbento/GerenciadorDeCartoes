using CardManager.API.Filters;
using CardManager.API.Middleware;
using CardManager.Application;
using CardManager.Infrastructure;
using CardManager.Infrastructure.Extensions;
using CardManager.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrateDatabase();

app.Run();


void MigrateDatabase()
{
    if (builder.Configuration.IsUniTestEnviromente())
        return;

    var databaseConnection = builder.Configuration.ConnectionString();
    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    DatabaseMigration.Migrate(databaseConnection, serviceScope.ServiceProvider);
}

public partial class Program { }