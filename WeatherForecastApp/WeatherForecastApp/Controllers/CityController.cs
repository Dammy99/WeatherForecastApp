using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text.Json;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class CityController : Controller
    {
        private readonly IConfiguration _configuration;
        //using 
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IJSRuntime _jsRuntime;
        public CityController(IConfiguration configuration, IJSRuntime jSRuntime)
        {
            _configuration = configuration;
            _jsRuntime = jSRuntime;
        }

        public async Task<IActionResult> Index()
        {
            string query = Request.Query["q"];

            if (string.IsNullOrEmpty(query))
            {
                return View("Index", query + " is not exists");
            }

            City city = new();

            await GetCityDataAsync(query, city);

            await GetCityWeatherForecastAsync(city);

            string cityJson = JsonConvert.SerializeObject(city);
            ViewBag.CityJson = cityJson;

            return View("Index", city);
        }

        public IActionResult Search()
        {
            return View();
        }

        private async Task GetCityDataAsync(string query, City city)
        {
            string apiUrl = $"http://dataservice.accuweather.com/locations/v1/cities/search?apikey={_configuration.GetValue<string>("AppSettings:ApiKey")}&q={query}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using (JsonDocument document = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = document.RootElement;
                    JsonElement first = root[0];
                    JsonElement key = first.GetProperty("Key");
                    JsonElement localizedName = first.GetProperty("LocalizedName");
                    JsonElement administrativeArea = first.GetProperty("AdministrativeArea").GetProperty("LocalizedName");
                    JsonElement country = first.GetProperty("Country").GetProperty("ID");

                    city.Key = key.ToString();
                    city.LocalizedName = localizedName.ToString();
                    city.AdministrativeArea = administrativeArea.ToString();
                    city.Country = country.ToString();
                }
            }
            else
            {
                throw new Exception("Помилка при запиті до API. Кількість пробних запитів на день завершилась");
            }
        }

        private async Task GetCityWeatherForecastAsync(City city)
        {
            string apiUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{city.Key}?apikey={_configuration.GetValue<string>("AppSettings:ApiKey")}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using (JsonDocument document = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = document.RootElement;
                    JsonElement maxTemperature = root.GetProperty("DailyForecasts")[0].GetProperty("Temperature").GetProperty("Maximum").GetProperty("Value");
                    JsonElement minTemperature = root.GetProperty("DailyForecasts")[0].GetProperty("Temperature").GetProperty("Minimum").GetProperty("Value");
                    JsonElement hasPresipitationDay = root.GetProperty("DailyForecasts")[0].GetProperty("Day").GetProperty("HasPrecipitation");
                    JsonElement hasPresipitationNight = root.GetProperty("DailyForecasts")[0].GetProperty("Night").GetProperty("HasPrecipitation");

                    if (hasPresipitationDay.GetBoolean())
                    {
                        JsonElement precipitationTypeDay = root.GetProperty("DailyForecasts")[0].GetProperty("Day").GetProperty("PrecipitationType");
                        city.PrecipitationTypeDay = precipitationTypeDay.ToString();
                    }

                    if (hasPresipitationNight.GetBoolean())
                    {
                        JsonElement precipitationTypeNight = root.GetProperty("DailyForecasts")[0].GetProperty("Night").GetProperty("PrecipitationType");
                        city.PrecipitationTypeNight = precipitationTypeNight.ToString();
                    }

                    city.MaxTemperature = Convert.ToInt32(Math.Round((maxTemperature.GetDouble() - 32) * 5 / 9));
                    city.MinTemperature = Convert.ToInt32(Math.Round((minTemperature.GetDouble() - 32) * 5 / 9));
                    city.HasPresipitationDay = hasPresipitationDay.GetBoolean();
                    city.HasPresipitationNight = hasPresipitationNight.GetBoolean();
                }
            }
            else
            {
                throw new Exception("Помилка при запиті до API. Кількість пробних запитів на день завершилась");
            }
        }
    }
}
