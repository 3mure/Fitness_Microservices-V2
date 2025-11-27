using Microsoft.Extensions.Logging;
using ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs;
using ProgressTrackingService.Shared;
using System.Net.Http.Json;

namespace ProgressTrackingService.Shared
{
    public class BmiServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<BmiServiceClient>? _logger;

        public BmiServiceClient(HttpClient http, ILogger<BmiServiceClient>? logger = null)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger;
            
            // Validate BaseAddress is set
            if (_http.BaseAddress == null)
            {
                throw new InvalidOperationException(
                    "HttpClient BaseAddress is not set. Please configure Services:FitnessCalculationService:BaseUrl in appsettings.json");
            }
            
            _logger?.LogInformation("BmiServiceClient initialized with BaseAddress: {BaseAddress}", _http.BaseAddress);
        }

        public async Task<double> GetBmiAsync(double weightInKg, double heightInCm)
        {
            try
            {
                if (_http.BaseAddress == null)
                {
                    throw new InvalidOperationException("HttpClient BaseAddress is not configured");
                }
                
                var url = $"/api/v1/calculations/bmi?weightInKg={weightInKg}&heightInCm={heightInCm}";
                var fullUrl = new Uri(_http.BaseAddress, url).ToString();
                _logger?.LogInformation("Calling FitnessCalculationService: {FullUrl}", fullUrl);

                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError("FitnessCalculationService returned error: {StatusCode} - {Content}", 
                        response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var result = await response.Content.ReadFromJsonAsync<FitnessCalculationServiceResponse<BmiResponseDto>>();

                if (result == null || !result.IsSuccess || result.Data == null)
                {
                    var errorMessage = result?.Message ?? "Unknown error from FitnessCalculationService";
                    _logger?.LogError("Failed to get BMI: {Message}", errorMessage);
                    throw new Exception($"Failed to calculate BMI: {errorMessage}");
                }

                _logger?.LogInformation("Successfully retrieved BMI: {BMI}", result.Data.BMI);
                return result.Data.BMI;
            }
            catch (HttpRequestException ex)
            {
                var baseUrl = _http.BaseAddress?.ToString() ?? "unknown";
                _logger?.LogError(ex, "HTTP error calling FitnessCalculationService at {BaseUrl}", baseUrl);
                
                if (ex.InnerException is System.Net.Sockets.SocketException socketEx && socketEx.ErrorCode == 10061)
                {
                    throw new Exception(
                        $"Failed to connect to FitnessCalculationService at {baseUrl}. " +
                        $"The service is not running or not accessible. " +
                        $"Please ensure FitnessCalculationService is running on the configured port. " +
                        $"Check the console output for the configured URL.", ex);
                }
                
                throw new Exception($"Failed to connect to FitnessCalculationService at {baseUrl}. Please ensure the service is running.", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calling FitnessCalculationService");
                throw;
            }
        }
    }
}
