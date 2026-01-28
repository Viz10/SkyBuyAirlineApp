using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.UI.Xaml;
using SkyBuy.Data.Data;
using SkyBuy.Services;
using SkyBuy.View.Navigations;
using SkyBuy.ViewModel;
using System;
using Windows.Media.Protection.PlayReady;

namespace SkyBuy
{
    public partial class App : Application
    {
        private Window _window;

        public static IConfiguration Configuration { get; private set; } = null!; /// Inverse of control with DI
        public static IServiceProvider Services { get; private set; } = null!;
            
        public static CustomerNavigation CustomerNavigationService { get; set; } = null!;
        public static AdminNavigation AdminNavigationService { get; set; } = null!;

        public App()
        {
            InitializeComponent();

            Configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false).Build();

            var services = new ServiceCollection(); /// the bucket

            /// DB
            services.AddDbContext<SkyBuyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            /// DB Parallel loads
            services.AddDbContextFactory<SkyBuyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            /// API GET ENDPOINTS
            services.AddHttpClient<AirLabsClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Airlabs:BaseUrl"]);
            });
            services.AddHttpClient<AirportsClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Airports:BaseUrl"]);
            });
            services.AddHttpClient<AeroDataBoxClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["AeroDataBox:BaseUrl"]);
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-market-key", Configuration["AeroDataBox:ApiKey"]);
            });

            services.AddSingleton<IConfiguration>(Configuration); /// json app present throught the program
           
            services.AddScoped<AuthenticationService>(); /// one per request 
            services.AddScoped<ScheduleService>();
            services.AddScoped<AirplaneFlightService>();

            /// a new one for each time needed
            services.AddTransient<LoginViewModel>(); 
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<CustomerMainDashBoardViewModel>();
            services.AddTransient<AdminLoginViewModel>();
            services.AddTransient<AdminMainDashBoardViewModel>();
            services.AddTransient<SchedulePlanificationViewModel>();
            services.AddTransient<FlightSelectionViewModel>();

            Services = services.BuildServiceProvider();
        }


        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
