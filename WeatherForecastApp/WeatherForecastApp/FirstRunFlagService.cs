namespace WeatherForecastApp
{
    public class FirstRunFlagService
    {
        private bool hasRun = false;

        public bool HasRun()
        {
            if (!hasRun)
            {
                hasRun = true;
                return false;
            }
            return true;
        }
    }
}
