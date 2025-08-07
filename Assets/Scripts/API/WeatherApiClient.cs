using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherApp.Data;
using WeatherApp.Config;

namespace WeatherApp.Services
{
    public class WeatherApiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(city))
            {
                Debug.LogError("City name cannot be empty.");
                return null;
            }

            // Get API key from config
            string apiKey = ApiConfig.OpenWeatherMapApiKey;
            if (!ApiConfig.IsApiKeyConfigured())
            {
                Debug.LogError("API key not configured. Please check config.json.");
                return null;
            }

            // Build full URL
            string url = $"{baseUrl}?q={UnityWebRequest.EscapeURL(city)}&appid={apiKey}&units=metric";
            Debug.Log($"[DEBUG] Final URL: {url}");

            // Make web request
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                // ✅ Handle ALL request failures
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"❌ Request failed: {request.result} - {request.error}\n{request.downloadHandler.text}");
                    return null;
                }

                // ✅ Parse JSON response
                try
                {
                    string json = request.downloadHandler.text;
                    WeatherData weather = JsonConvert.DeserializeObject<WeatherData>(json);
                    return weather;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Error parsing weather data: {e.Message}");
                    return null;
                }
            }
        }

        // Example usage on start
        private async void Start()
        {
            var weatherData = await GetWeatherDataAsync("London");

            if (weatherData != null && weatherData.IsValid)
            {
                Debug.Log($"✅ Weather in {weatherData.CityName}: {weatherData.TemperatureInCelsius:F1}°C");
                Debug.Log($"🌤️ Description: {weatherData.PrimaryDescription}");
            }
            else
            {
                Debug.LogError("❌ Failed to get weather data");
            }
        }
    }
}
