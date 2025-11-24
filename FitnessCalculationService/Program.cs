using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using FitnessCalculationService.Domain.Entities;
using FitnessCalculationService.Domain.Interfaces;
using FitnessCalculationService.Features;
using FitnessCalculationService.Infrastructure.Data;
using FitnessCalculationService.Infrastructure.Repositories;
using FitnessCalculationService.Infrastructure.UnitOfWork;
using WorkoutService.Features;

namespace FitnessCalculationService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Serilog setup
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "FitnessCalculationService")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
                .WriteTo.File("logs/FitnessCalculationService-.log", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting FitnessCalculationService");

                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                var config = builder.Configuration;

                // Add Services to the container

                // Register Memory Cache
                builder.Services.AddMemoryCache();

                // Add DBContext for SQL Server
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                    if (builder.Environment.IsDevelopment())
                    {
                        options.EnableSensitiveDataLogging(true);
                        options.EnableDetailedErrors(true);
                    }
                });

                // Register Unit of Work
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                // Register Generic Repositories
                var entityTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)))
                    .ToList();

                foreach (var entityType in entityTypes)
                {
                    var interfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);
                    var implementationType = typeof(BaseRepository<>).MakeGenericType(entityType);
                    builder.Services.AddScoped(interfaceType, implementationType);
                }

                Log.Information("Registered {Count} generic repositories", entityTypes.Count);

                // Add MediatR
                builder.Services.AddMediatR(typeof(Program).Assembly);

                // Add Mapster
                var typeAdapterConfig = Mapster.TypeAdapterConfig.GlobalSettings;
                typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
                builder.Services.AddSingleton(typeAdapterConfig);

                // Add CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        b => b.AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials());
                });

                // Add Authentication
                builder.Services.AddAuthentication(options =>
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
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                    };
                });

                builder.Services.AddAuthorization();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Fitness Calculation Service API", Version = "v1" });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter JWT with Bearer into field (e.g., 'Bearer {token}')",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new string[] {}
                        }
                    });
                });

                var app = builder.Build();

                // Database Migration
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        await context.Database.MigrateAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error during migration");
                        if (app.Environment.IsDevelopment()) throw;
                    }
                }

                // Configure the HTTP request pipeline

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapAllEndpoints();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
