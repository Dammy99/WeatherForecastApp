namespace WeatherForecastApp.Models
{
    public class City
    {
        public string? Key { get; set; }
        public string? LocalizedName { get; set; }
        public string? AdministrativeArea { get; set; }
        public string? Country { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public bool HasPresipitationDay { get; set; }
        public bool HasPresipitationNight { get; set; }
        public string? PrecipitationTypeDay { get; set; }
        public string? PrecipitationTypeNight { get; set; }
    }
}
