using InventoryService.MessageBroker;
using MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Infrastructure.UnitOfWork;
using ProgressTrackingService;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Infrastructure;
using ProgressTrackingService.Infrastructure.Data;
using ProgressTrackingService.Shared;
using ProgressTrackingService.MessageBroker;

namespace ProgressTrackingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Configure HttpClient for FitnessCalculationService
            var fitnessCalculationServiceUrl = builder.Configuration["Services:FitnessCalculationService:BaseUrl"] 
                ?? "http://localhost:5275";
            
            if (string.IsNullOrWhiteSpace(fitnessCalculationServiceUrl))
            {
                throw new InvalidOperationException("FitnessCalculationService BaseUrl is not configured. Please set Services:FitnessCalculationService:BaseUrl in appsettings.json");
            }
            
            Console.WriteLine($"[ProgressTrackingService] Configuring FitnessCalculationService URL: {fitnessCalculationServiceUrl}");
            
            builder.Services.AddHttpClient<BmiServiceClient>(client =>
            {
                client.BaseAddress = new Uri(fitnessCalculationServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            // Note: AddHttpClient<T> automatically registers T, so we don't need AddScoped
            builder.Services.AddSingleton<IMessageBrokerPublisher>(provider =>
            {
                return new MessageBrokerPublisher(provider.GetRequiredService<IConfiguration>());
            }); 

            builder.Services.AddScoped<IUniteOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(BaseRepository<>));
            builder.Services.AddDbContext<FitnessAppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging(true);
                    options.EnableDetailedErrors(true);
                }
            });
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "redis-18828.c16.us-east-1-3.ec2.redns.redis-cloud.com:18828,password=sKd8WfR2OctEuTiPXH5iqeQjS75xFlkl";
            //    options.InstanceName = "FitnessApp";
            //});
            builder.Services.AddMemoryCache();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddMediatR(typeof(Program).Assembly);

            // Register RabbitMQ Consumer Service as a hosted service
            builder.Services.AddHostedService<RabbitMQConsumerService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Fix: Move summaries declaration outside of MapGet and use correct chaining
            string[] summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            //app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            //{
            //    var forecast = Enumerable.Range(1, 5).Select(index =>
            //        new WeatherForecast
            //        {
            //            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //            TemperatureC = Random.Shared.Next(-20, 55),
            //            Summary = summaries[Random.Shared.Next(summaries.Length)]
            //        })  
            //        .ToArray();
            //    return forecast;
            //})
            //.WithName("GetWeatherForecast")
            //.WithOpenApi();

            app.Run();
        }
    }
}
