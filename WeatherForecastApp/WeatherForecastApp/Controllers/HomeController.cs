using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IJSRuntime _jsRuntime;
        private readonly FirstRunFlagService _firstRunFlagService;
        private readonly List<City> _cities;
        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration,
            IJSRuntime jsRuntime,
            FirstRunFlagService firstRunFlagService)
        {
            _logger = logger;
            _configuration = configuration;
            _jsRuntime = jsRuntime;
            _firstRunFlagService = firstRunFlagService;
        }

        public IActionResult Index()
        {
            ViewBag.IsFirstRun = JsonConvert.SerializeObject(!_firstRunFlagService.HasRun());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}