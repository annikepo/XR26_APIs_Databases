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
        [SerializeField] private string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Debug.LogError("City name cannot be empty");
                return null;
            }

            string apiKey = ApiConfig.OpenWeatherMapApiKey;
            if (!ApiConfig.IsApiKeyConfigured())
            {
                Debug.LogError("API key not configured. Check config.json.");
                return null;
            }

            string url = $"{baseUrl}?q={UnityWebRequest.EscapeURL(city)}&appid={apiKey}&units=metric";
            Debug.Log($"[DEBUG] Final URL: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        return ParseWeatherData(request.downloadHandler.text);

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError($"🌐 Network connection error: {request.error}");
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"📡 HTTP error {request.responseCode}: {request.downloadHandler.text}");
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"📦 Data processing error: {request.error}");
                        break;

                    default:
                        Debug.LogError($"❌ Unknown error: {request.error}");
                        break;
                }

                return null;
            }
        }

        private WeatherData ParseWeatherData(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                return JsonConvert.DeserializeObject<WeatherData>(json, settings);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"❌ JSON parsing failed: {ex.Message}");
                return null;
            }
        }
    }
}
